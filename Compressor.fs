//TODO: add check for empty array in CompressRLE 

namespace POAS

open System

/// Contains methods to compress a data with a various algorithms.
type Compressor = 
    static member private AddBit (container : ResizeArray<byte>) residue bit =
        let tmp = Convert.ToString(container.[container.Count-1], 2)
        let binstr = (new String('0', 8-tmp.Length) + tmp).Substring(0, 8-residue) + bit + new String('0', residue-1)
        container.[container.Count-1] <- container.[container.Count-1] ||| Convert.ToByte(binstr, 2)
        if residue = 1 then
            container.Add(0uy)
            8
        else
            residue-1
    
    /// Performs a compressing with RLE algorithm.
    static member CompressRLE (data : byte[]) =
        let result = new ResizeArray<byte>()

        // Recursively processes a data for forming the result.
        let rec processing i current count =
            if i < data.Length then
                if i = 0 then
                    processing 1 data.[i] 1y
                elif data.[i] = current then
                    if count = 127y then
                        result.Add(byte count)
                        result.Add(current)
                        processing (i+1) current 1y
                    elif count < 0y then
                        processing (i+1) current 2y
                    else
                        processing (i+1) current (count+1y)
                else
                    if count > 1y then
                        result.Add(byte count)
                        result.Add(current)
                        processing (i+1) data.[i] 1y
                    elif count = 1y then
                        result.Add(byte count)
                        result.Add(current)
                        processing (i+1) data.[i] -2y
                    elif count = -128y then
                        result.[result.Count-128] <- byte count
                        result.Add(current)
                        processing (i+1) data.[i] 1y
                    else
                        result.[result.Count+(int count)] <- byte count
                        result.Add(current)
                        processing (i+1) data.[i] (count-1y)
            else
                if i <> 0 then
                    if count > 0y then
                        result.Add(byte count)
                    else
                        result.[result.Count+(int count)] <- byte count

                    result.Add(current)
      
        processing 0 0uy 0y // Start processing from the first byte of data.
        result.ToArray()

    /// Performs a decompressing with RLE algorithm.
    static member DecompressRLE (data : byte[]) =
        // Count of bytes must be even.
        try
            let result = new ResizeArray<byte>()

            // Recursively processes a data for forming the result.
            let rec processing i =
                if i < data.Length then          
                    if data.[i] = 0uy then
                        processing (i+2)
                    elif data.[i] < 128uy then
                        result.AddRange(seq {for j=1 to (int data.[i]) do yield data.[i+1]})
                        processing (i+2)
                    else
                        result.AddRange(seq {for j=i+1 to i-(int (sbyte data.[i])) do yield data.[j]})
                        processing <| i+1+(abs (int (sbyte data.[i])))
   
            processing 0 // Start processing from the first byte of data.
            result.ToArray()
        with
            | :? IndexOutOfRangeException -> raise (new ArgumentException "Data is corrupted.")
    
    /// Performs a compressing with adaptive Huffman coding.
    static member CompressHuffman (data: byte[]) =
        let result = new ResizeArray<byte>()

        // Table of bytes frequences.
        let table = ref Array.empty
        
        if data.Length = 1 then
            result.Add(data.[0])
            result.Add(0uy)
            Compressor.AddBit result 8 "1" |> ignore
            Compressor.AddBit result 7 "1" |> ignore
        elif data.Length > 1 then
            table.Value <- Array.append [|data.[0], 1|] !table // Add first byte to table.
            result.Add(data.[0])    // First byte is adding directly.
            result.Add(0uy)         // Create space for next codes.
            let mutable residue = 8 // Now we have 8 free bits of space.

            // Loop to process rest bytes.
            for i=1 to data.Length-1 do
                let index = Array.tryFindIndex (fun iter -> fst iter = data.[i]) !table
                if index.IsSome then     // If current byte is in table...
                    let cnt = Seq.findIndex (fun iter -> fst iter = data.[i]) !table
                    let binstr = new String('1', cnt) + "0" //!!
                    for c in binstr do
                        residue <- Compressor.AddBit result residue (c.ToString()) // And add them to output array.
                    table.Value <- Array.append !table [|data.[i], (snd (!table).[index.Value] + 1)|]  // Increment a frequency.
                    table.Value <- Array.filter (fun iter -> iter <> (!table).[index.Value]) !table
                    table.Value <- (Array.sortBy (fun iter -> -(snd iter)) !table)
                else
                    // Cumpute the ESC code for new byte.
                    let tmp = Convert.ToString(data.[i], 2)
                    let binstr = new String('1', (!table).Length) + "0" + new String('0', 8-tmp.Length) + tmp
                    for c in binstr do
                        residue <- Compressor.AddBit result residue (c.ToString()) // And add them to output array.
                    table.Value <- Array.append !table [|data.[i], 1|]

            let binstr = new String('1', (!table).Length) + "1"
            for c in binstr do
                residue <- Compressor.AddBit result residue (c.ToString()) // And add them to output array.

        result.ToArray()
