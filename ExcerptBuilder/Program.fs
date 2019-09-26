open System.Text
open PdfSharp.Pdf
open PdfSharp.Pdf.IO
open Argu
open PdfSharp.Drawing
open PdfSharp.Pdf.Annotations

type CliArguments =
    | First_Page of number:int
    | Last_Page of number:int
    | Original_Path of path:string
    | Destination_Path of path:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | First_Page _ -> "specify a first page"
            | Last_Page _ -> "specify a last page"
            | Original_Path _ -> "specify a original document path"
            | Destination_Path _ -> "specify a destination document path"
            
            
[<EntryPoint>]
let main argv =
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
 
    let addAnnotations page =
        let gfx = XGraphics.FromPdfPage(page)
        let font = new XFont("Verdana", 20.0, XFontStyle.Underline)
        gfx.DrawString("The first text annotation", font, XBrushes.Black, 30.0, 50.0, XStringFormats.Default);
        let rect = gfx.Transformer.WorldToDefaultPage(new XRect(new XPoint(30.0, 50.0), new XSize(30.0, 50.0)))
        let link = PdfLinkAnnotation.CreateWebLink(new PdfRectangle(rect), "http://localhost:3000")
        page.Annotations.Add(link)

    let parser = ArgumentParser.Create<CliArguments>(programName = "excerptbuilder.exe")
    let options = parser.ParseCommandLine argv
    
    let originalPath = options.GetResult Original_Path
    let destinationPath = options.GetResult Destination_Path
    
    let originalPdf = PdfReader.Open(originalPath, PdfDocumentOpenMode.Import)
    
    let first = options.GetResult First_Page
    let last = if originalPdf.Pages.Count < options.GetResult(Last_Page) then originalPdf.Pages.Count else options.GetResult(Last_Page)
    
    let excerpt = new PdfDocument()
    for i in first .. last do
        let page = originalPdf.Pages.Item(i - 1)
        excerpt.AddPage(page) |> ignore
           
            
    addAnnotations(excerpt.Pages.Item(last - 1))
        
    
    excerpt.Save(destinationPath)
    
    0 // return an integer exit code
