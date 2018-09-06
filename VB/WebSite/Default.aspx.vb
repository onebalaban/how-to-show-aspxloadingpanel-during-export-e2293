Imports System
Imports System.Data
Imports System.Web
Imports System.IO
Imports DevExpress.Web
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrintingLinks

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        grid.AutoGenerateColumns = True
        grid.Width = 400
        grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto
        grid.SettingsExport.EnableClientSideExportAPI = True
        grid.DataSource = GetDataTable()
        grid.DataBind()
    End Sub
    Private Function GetDataTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("NumberID", System.Type.GetType("System.Int32"))
        For i As Integer = 0 To 9
            dt.Columns.Add("Number" & i, System.Type.GetType("System.Int32"))
        Next i
        dt.PrimaryKey = New DataColumn() { dt.Columns("NumberID") }
        For i As Integer = 0 To 999
            dt.Rows.Add(i, i, i, i, i, i, i, i, i, i, i)
        Next i
        Return dt
    End Function
    Protected Sub bnExport_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As ASPxButton = TryCast(sender, ASPxButton)

        Dim ps As New PrintingSystemBase()
        Dim lnk As New PrintableComponentLinkBase(ps)
        lnk.Component = gridExporter

        Dim compositeLink As New CompositeLinkBase(ps)
        compositeLink.Links.AddRange(New Object() { lnk })
        compositeLink.CreateDocument()

        Dim stream As New MemoryStream()
        Dim type As String = String.Empty
        Select Case btn.ID
            Case "bnExportPDF"
                compositeLink.PrintingSystemBase.ExportToPdf(stream)
                type = "pdf"
            Case "bnExportXLS"
                compositeLink.PrintingSystemBase.ExportToXls(stream)
                type = "xls"
            Case "bnExportRRF"
                compositeLink.PrintingSystemBase.ExportToRtf(stream)
                type = "rtf"
        End Select
        Session("ExportStreame") = stream
        Session("type") = type
    End Sub
    Protected Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim stream As MemoryStream = TryCast(Session("ExportStreame"), MemoryStream)
        Dim type As String = Session("type").ToString()
        WriteToResponse(grid.ID, True, type, stream)
    End Sub
    Protected Sub WriteToResponse(ByVal fileName As String, ByVal saveAsFile As Boolean, ByVal fileFormat As String, ByVal stream As MemoryStream)
        If Page Is Nothing OrElse Page.Response Is Nothing Then
            Return
        End If
        Dim disposition As String = If(saveAsFile, "attachment", "inline")
        Page.Response.Clear()
        Page.Response.Buffer = False
        Page.Response.AppendHeader("Content-Type", String.Format("application/{0}", fileFormat))
        Page.Response.AppendHeader("Content-Transfer-Encoding", "binary")
        Page.Response.AppendHeader("Content-Disposition", String.Format("{0}; filename={1}.{2}", disposition, HttpUtility.UrlEncode(fileName).Replace("+", "%20"), fileFormat))
        Page.Response.BinaryWrite(stream.ToArray())
        Page.Response.End()
    End Sub
End Class