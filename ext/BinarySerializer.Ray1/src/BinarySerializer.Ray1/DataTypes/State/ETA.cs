using System.Linq;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Object states
    /// </summary>
    public class ETA : BinarySerializable
    {
        /// <summary>
        /// The number of Etats
        /// </summary>
        public long? EtatCount { get; set; }

        /// <summary>
        /// The numbers of SubEtats
        /// </summary>
        public long[] SubEtatCount { get; set; }

        public Pointer[] EtatPointers { get; set; }

        /// <summary>
        /// Collection of states and substates
        /// </summary>
        public ObjState[][] States { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetRequiredSettings<Ray1Settings>();

            // Get number of ETAs, hack
            if (EtatCount == null)
            {
                // Save the current pointer
                var pointer = s.CurrentPointer;

                // Get the number of etats
                s.DoAt(pointer, () => 
                {
                    Pointer p = s.SerializePointer(null, name: "FirstEtat");
                    
                    if (p.File != pointer.File || 
                        p.AbsoluteOffset < pointer.AbsoluteOffset + 4 || 
                        (p.AbsoluteOffset - pointer.AbsoluteOffset) % 4 != 0)
                        s.Context.SystemLogger?.LogWarning("Number of ETAs wasn't correctly determined");

                    EtatCount = (p.AbsoluteOffset - pointer.AbsoluteOffset) / 4;
                });
            }

            // Serialize the Etat pointers
            EtatPointers = s.SerializePointerArray(EtatPointers, EtatCount ?? 0, name: nameof(EtatPointers));
            
            // Get number of SubEtats for each Etat
            if (SubEtatCount == null)
            {
                // Get the state size
                uint stateSize = settings.EngineVersion switch
                {
                    Ray1EngineVersion.R2_PS1 => 16u,
                    Ray1EngineVersion.PS1_JPDemoVol3 => 14u,
                    Ray1EngineVersion.PS1_JPDemoVol6 => 12u,
                    _ => 8
                };

                SubEtatCount = new long[EtatCount ?? 0];
                
                // Enumerate every Etat, except last one
                for (int i = 0; i < EtatPointers.Length - 1; i++)
                {
                    // Make sure we have a valid pointer
                    if (EtatPointers[i] == null) 
                        continue;
                    
                    // Get size and make sure the next one is not null
                    if (EtatPointers[i + 1] != null)
                        SubEtatCount[i] = (EtatPointers[i + 1].AbsoluteOffset - EtatPointers[i].AbsoluteOffset) / stateSize;
                    else
                        s.Context.SystemLogger?.LogWarning("An Etat Pointer was null - Number of SubEtats couldn't be determined");
                }

                // Get the size of the last Etat
                if (EtatPointers[EtatCount.Value - 1] != null)
                {
                    // TODO: Find better way to parse this

                    s.DoAt(EtatPointers.Last(), () =>
                    {
                        uint count = 0;
                        const int maxCount = 69;

                        while (true)
                        {
                            // Make sure we can read more
                            if (s.CurrentLength - stateSize < s.CurrentPointer.FileOffset)
                                break;

                            // Make sure it's not a pointer
                            if (s.DoAt(s.CurrentPointer, () => s.SerializePointer(default, allowInvalid: true, name: $"Dummy pointer {count}")) != null)
                                break;

                            // Read the next state
                            ObjState state = s.SerializeObject<ObjState>(null, name: $"Dummy state {count}");

                            // Additional check to make sure only valid state data gets read
                            if (state.NextMainEtat >= EtatPointers.Length &&
                                // The vol 3 demo has some states with garbage data before valid ones, so don't include this
                                settings.EngineVersion != Ray1EngineVersion.PS1_JPDemoVol3 &&
                                // Rayman 2 uses 0xFF for the last state in a link-chain for when always objects should be deleted
                                !(state.NextMainEtat == 0xFF && settings.EngineVersion == Ray1EngineVersion.R2_PS1))
                                break;

                            // Make sure we haven't reached the max
                            if (count > maxCount)
                                break;

                            count++;
                        }

                        SubEtatCount[EtatCount.Value - 1] = count;
                    });
                }
            }

            // Create state array
            States ??= new ObjState[EtatCount ?? 0][];

            // Serialize the states
            for (int i = 0; i < EtatPointers.Length; i++)
                s.DoAt(EtatPointers[i], () => States[i] = s.SerializeObjectArray<ObjState>(States[i], SubEtatCount[i], name:
                    $"{nameof(States)}[{i}]"));
        }
    }
}