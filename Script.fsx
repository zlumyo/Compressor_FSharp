
#load "Compressor.fs"

open POAS

open System

/// Shows a messagebox about occured error.
let DisplayError message =
    ignore <| System.Windows.Forms.MessageBox.Show("An exception has occured:" + Environment.NewLine + message, "UnitTesting")

/// Compares two arrays.
let CompareArrays a b = 
    Array.forall2 (fun iter1 iter2 -> iter1 = iter2) a b

// UnitTesing for RLE compression.

try
    let tests_for_rle_compression = 
        [
            "AAAAA"; "AAAAAB"; "ABBBBB"; "AAABCCC"; "ABCDE"; "ABCDEEEE"; new String('A', 128);
            (* 60 chars + 60 chars + 9 chars*) "ABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABC" + 
            "ABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABC" + "ABCABCABC"; ""
        ]

    let etalons_for_rle_compression = 
        [
            [| 5uy; 65uy |] ; [| 5uy; 65uy; 1uy; 66uy |] ; [| 1uy; 65uy; 5uy; 66uy |] ; [| 3uy; 65uy; 1uy; 66uy; 3uy; 67uy |] ;
            [| 251uy; 65uy; 66uy; 67uy; 68uy; 69uy |] ; [| 252uy; 65uy; 66uy; 67uy; 68uy; 4uy; 69uy |] ;
            [| 127uy; 65uy; 1uy; 65uy|] ; [| 128uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 
                                                    65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 1uy; 67uy |] ; [| |]
        ]

    let encoder = new System.Text.ASCIIEncoding()

    for i=0 to tests_for_rle_compression.Length-1 do
        if not <| CompareArrays etalons_for_rle_compression.[i] (Compressor.CompressRLE <| encoder.GetBytes tests_for_rle_compression.[i]) then
            System.Windows.Forms.MessageBox.Show(String.Format("In {0} test for RLE compression coding fail has occured.", i+1)) |> ignore
with
    | ex -> DisplayError ex.Message

// UnitTesing for RLE decompression.

try
    let tests_for_rle_decompression = 
        [
            [| |] ; [| 0uy; 65uy |] ; [| 255uy; 65uy |] ; [| 3uy; 65uy |] ; [| 254uy; 65uy; 66uy |] ;
            [| 254uy; 65uy; 66uy; 3uy; 65uy |] ; [| 3uy; 65uy; 254uy; 65uy; 66uy |] ; [| 1uy |] ; [| 254uy; 65uy |]
        ]

    let etalons_for_rle_decompression = 
        [
            [| |] ; [| |] ; [| 65uy |] ; [| 65uy; 65uy; 65uy |] ; [| 65uy; 66uy |] ; [| 65uy; 66uy; 65uy; 65uy; 65uy; |] ;
            [| 65uy; 65uy; 65uy; 65uy; 66uy |] ; [| |] ; [| |]
        ]

    for i=0 to tests_for_rle_decompression.Length-1 do
        try
            if not <| CompareArrays etalons_for_rle_decompression.[i] (Compressor.DecompressRLE tests_for_rle_decompression.[i]) then
                System.Windows.Forms.MessageBox.Show(String.Format("In {0} test for RLE decompression fail has occured.", i+1)) |> ignore
        with
            | :? ArgumentException -> ()
with
    | ex -> DisplayError ex.Message

// UnitTesting for Huffman coding.

try
    let tests_for_huffman_coding = 
        [
            [| |] ; [| 65uy |] ; [| 65uy; 65uy |] ; [| 65uy; 66uy |] ; [| 65uy; 66uy; 66uy |] ; [| 65uy; 66uy; 65uy; 65uy |]
        ]

    let etalons_for_huffman_coding = 
        [
            [| |] ; [| 65uy; 192uy |] ; [| 65uy; 96uy |] ; [| 65uy; 144uy; 184uy |] ; [| 65uy; 144uy; 174uy |] ;
            [| 65uy; 144uy; 142uy |]
        ]

    for i=0 to tests_for_huffman_coding.Length-1 do
        if not <| CompareArrays etalons_for_huffman_coding.[i] (Compressor.CompressHuffman tests_for_huffman_coding.[i]) then
            System.Windows.Forms.MessageBox.Show(String.Format("In {0} test for Huffman coding fail has occured.", i+1)) |> ignore
with
    | ex -> DisplayError ex.Message

System.Windows.Forms.MessageBox.Show("UnitTesting was completed.", "UnitTesting") |> ignore