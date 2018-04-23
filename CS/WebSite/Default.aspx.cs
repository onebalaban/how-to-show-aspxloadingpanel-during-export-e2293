using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxGridView.Export.Helper;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System.IO;
using DevExpress.Web.ASPxEditors;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e){
        grid.AutoGenerateColumns = true;
        grid.Width = 400;
        grid.Settings.ShowHorizontalScrollBar = true;

        grid.DataSource = GetDataTable();
        grid.DataBind();
    }
    protected void Page_Load(object sender, EventArgs e){

    }

    private DataTable GetDataTable(){

        DataTable dt = new DataTable();
        dt.Columns.Add("NumberID", System.Type.GetType("System.Int32"));
        for (int i = 0; i < 10; i++){
            dt.Columns.Add("Number" + i, System.Type.GetType("System.Int32"));
        }
        dt.Columns.Add("Number", System.Type.GetType("System.Int32"));
        dt.PrimaryKey = new DataColumn[] { dt.Columns["NumberID"] };


        for (int i = 0; i < 1000; i++){
            dt.Rows.Add(i, i, i, i, i, i, i, i, i, i);
        }

        return dt;
    }

    protected void bnExport_Click(object sender, EventArgs e){
        ASPxButton btn = sender as ASPxButton;
        GridViewLink lnk = new GridViewLink(gridExporter);
        PrintingSystem ps = lnk.CreatePS();

        MemoryStream stream = new MemoryStream();
        string type = string.Empty;
        switch (btn.ID){
            case "bnExportPDF":
                ps.ExportToPdf(stream);
                type = "pdf";
                break;
            case "bnExportXLS":
                ps.ExportToXls(stream);
                type = "xls";
                break;
            case "bnExportRRF":
                ps.ExportToRtf(stream);
                type = "rtf";
                break;
        }
        Session["ExportStreame"] = stream;
        Session["type"] = type;
    }
    protected void btn_Click(object sender, EventArgs e){
        MemoryStream stream = Session["ExportStreame"] as MemoryStream;
        string type = Session["type"].ToString();
        WriteToResponse(grid.ID, true, type, stream);
    }
    protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, MemoryStream stream){
        byte[] mem = stream.GetBuffer();
        if (Page == null || Page.Response == null) return;
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