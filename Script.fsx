
#load "Compressor.fs"
open POAS

open System

let tests_for_rle_compression = 
    [
        "AAAAA"; "AAAAAB"; "ABBBBB"; "AAABCCC"; "ABCDE"; "ABCDEEEE"; new String('A', 128);
        (* 60 chars + 60 chars + 9 chars*) "ABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABC" + 
        "ABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABCABC" + "ABCABCABC"
    ]

let etalons_for_rle_compression = 
    [
        [| 5uy; 65uy |] ; [| 5uy; 65uy; 1uy; 66uy |] ; [| 1uy; 65uy; 5uy; 66uy |] ; [| 3uy; 65uy; 1uy; 66uy; 3uy; 67uy; |] ;
        [| 1uy; 65uy; 1uy; 66uy; 1uy; 67uy; 1uy; 68uy; 1uy; 69uy; |] ; [| 1uy; 65uy; 1uy; 66uy; 1uy; 67uy; 1uy; 68uy; 4uy; 69uy; |] ;
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
                                                65uy; 66uy; 67uy; 65uy; 66uy; 67uy; 65uy; 66uy; 1uy; 67uy; |]
    ]

let encoder = new System.Text.ASCIIEncoding()

for i=0 to tests_for_rle_compression.Length-1 do
    assert (etalons_for_rle_compression.[i].Equals(Compressor.CompressRLE <| encoder.GetBytes tests_for_rle_compression.[i]))

System.Windows.Forms.MessageBox.Show("UnitTesting was completed.", "UnitTesting")