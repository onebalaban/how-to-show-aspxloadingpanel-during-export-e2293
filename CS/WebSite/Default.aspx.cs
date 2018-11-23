using System;
using System.Data;
using System.Web;
using System.IO;
using DevExpress.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Init(object sender, EventArgs e) {
        grid.AutoGenerateColumns = true;
        grid.Width = 400;
        grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
        grid.SettingsExport.EnableClientSideExportAPI = true;
        grid.DataSource = GetDataTable();
        grid.DataBind();
    }
    private DataTable GetDataTable() {
        DataTable dt = new DataTable();
        dt.Columns.Add("NumberID", System.Type.GetType("System.Int32"));
        for(int i = 0; i < 10; i++) {
            dt.Columns.Add("Number" + i, System.Type.GetType("System.Int32"));
        }
        dt.PrimaryKey = new DataColumn[] { dt.Columns["NumberID"] };
        for(int i = 0; i < 1000; i++) {
            dt.Rows.Add(i, i, i, i, i, i, i, i, i, i, i);
        }
        return dt;
    }
    protected void bnExport_Click(object sender, EventArgs e) {
        ASPxButton btn = sender as ASPxButton;

        PrintingSystemBase ps = new PrintingSystemBase();
        PrintableComponentLinkBase lnk = new PrintableComponentLinkBase(ps);
        lnk.Component = gridExporter;

        CompositeLinkBase compositeLink = new CompositeLinkBase(ps);
        compositeLink.Links.AddRange(new object[] { lnk });
        compositeLink.CreateDocument();

        MemoryStream stream = new MemoryStream();
        string type = string.Empty;
        switch(btn.ID) {
            case "bnExportPDF":
                compositeLink.PrintingSystemBase.ExportToPdf(stream);
                type = "pdf";
                break;
            case "bnExportXLS":
                compositeLink.PrintingSystemBase.ExportToXls(stream);
                type = "xls";
                break;
            case "bnExportRRF":
                compositeLink.PrintingSystemBase.ExportToRtf(stream);
                type = "rtf";
                break;
        }
        Session["ExportStream"] = stream;
        Session["type"] = type;
    }
    protected void btn_Click(object sender, EventArgs e) {
        MemoryStream stream = Session["ExportStream"] as MemoryStream;
        string type = Session["type"].ToString();
        WriteToResponse(grid.ID, true, type, stream);
    }
    protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, MemoryStream stream) {
        if(Page == null || Page.Response == null) return;
        string disposition = saveAsFile ? "attachment" : "inline";
        Page.Response.Clear();
        Page.Response.Buffer = false;
        Page.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat));
        Page.Response.AppendHeader("Content-Transfer-Encoding", "binary");
        Page.Response.AppendHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", disposition, HttpUtility.UrlEncode(fileName).Replace("+", "%20"), fileFormat));
        Page.Response.BinaryWrite(stream.ToArray());
        Page.Response.End();
    }
}