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
            
type Builder(options: ParseResults<CliArguments>) =
     member this.originalPath = options.GetResult Original_Path
     member this.destinationPath = options.GetResult Destination_Path
     member this.originalPdf = PdfReader.Open(this.originalPath, PdfDocumentOpenMode.Import)
     member this.first = options.GetResult First_Page
     member this.last =
         let count = this.originalPdf.Pages.Count
         if  count < options.GetResult(Last_Page) then
             count
         else
            options.GetResult(Last_Page)
            
     member this.Build() =
        let excerpt = new PdfDocument()
        for i in this.first .. this.last do
            let page = this.originalPdf.Pages.Item(i - 1)
            excerpt.AddPage(page) |> ignore
        this.addAnnotations(excerpt.Pages.Item(this.last - 1))
        excerpt.Save(this.destinationPath)
        
     member this.addAnnotations page =
         let gfx = XGraphics.FromPdfPage(page)
         let font = new XFont("Verdana", 20.0, XFontStyle.Underline)
         gfx.DrawString("The first text annotation. Davai.", font, XBrushes.Black, 30.0, 50.0, XStringFormats.Default);
         let rect = gfx.Transformer.WorldToDefaultPage(new XRect(new XPoint(30.0, 50.0), new XSize(30.0, 50.0)))
         let link = PdfLinkAnnotation.CreateWebLink(new PdfRectangle(rect), "http://localhost:3000")
         page.Annotations.Add(link)
      
            
[<EntryPoint>]
let main argv =
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        
    let parser = ArgumentParser.Create<CliArguments>(programName = "excerptbuilder.exe")
    let options = parser.ParseCommandLine argv
    
    let builder = new Builder(options)
    builder.Build()
    
    0 // return an integer exit code
