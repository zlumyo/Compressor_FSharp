namespace POAS

/// Contains methods to compress a data with a various algorithms.
type Compressor = 
    /// Performs a compressing with RLE algorithm.
    static member CompressRLE (data : byte[]) =
        let result = new ResizeArray<byte>()

        // Recursively processes a data for forming the result.
        let rec processing i current count =
            if i = 0 then
                processing 1 data.[i] 1y
            elif i < data.Length then
                if data.[i] = current then
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
                if count > 0y then
                    result.Add(byte count)
                else
                    result.[result.Count+(int count)] <- byte count

                result.Add(current)
      
        processing 0 0uy 0y // Start processing from the first byte of data.
        result.ToArray()
