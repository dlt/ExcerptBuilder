open System.Text
open PdfSharp.Pdf
open PdfSharp.Pdf.IO
open Argu

type CliArguments =
    | First_Page of number:int
    | Last_Page of number:int
    | Original_Path of path:string
    | Destination_path of path:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | First_Page _ -> "specify a first page"
            | Last_Page _ -> "specify a last page"
            | Original_Path _ -> "specify a original document path"
            | Destination_path _ -> "specify a result document path"
            
let b = "/home/dlt/RubymineProjects/balzac/public/5d582889a6654f29036d.pdf"
let d = "/home/dlt/balzac-exerpt.pdf"

[<EntryPoint>]
let main argv =
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
    
    let parser = ArgumentParser.Create<CliArguments>(programName = "chunkbuilder.exe")
    
    let options = parser.ParseCommandLine argv
    let first = options.GetResult First_Page
    let last = options.GetResult Last_Page
    let originalPath = options.GetResult Original_Path
    let destinationPath = options.GetResult Original_Path
    
    let pdf = PdfReader.Open(originalPath, PdfDocumentOpenMode.Import)
    let filename = destinationPath
    let doc = new PdfDocument()
    
    for i in first .. last do
        doc.AddPage(pdf.Pages.Item(i)) |> ignore
    
    doc.Save(filename)
    
    0 // return an integer exit code
