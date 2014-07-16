<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Plugghest.Modules.DisplayPlugg.View" %>


<asp:Panel ID="pnlDisplayInfo" runat="server">
    <asp:HyperLink ID="hlDisplayInfo" style="font-size: xx-small; float: right;" runat="server" resourcekey="DisplayInfo" /><br />
</asp:Panel>

<asp:Panel ID="pnlHideDisplayInfo" runat="server" Visible ="false">
    <asp:HyperLink ID="hlHideDisplayInfo" style="font-size: xx-small; float: right;" runat="server" resourcekey="HideDisplayInfo" /><br />
</asp:Panel>

<asp:Panel ID="pnlToCreationLanguage" runat="server" Visible ="false">
    <asp:HyperLink ID="hlToCreationLanguage" style="font-size: xx-small; float: right;" runat="server" resourcekey="ToCreationLanguage" /><br />
</asp:Panel>

<asp:Panel ID="pnlTranslatePlugg" runat="server" Visible ="false">
    <asp:HyperLink ID="hlTranslatePlugg" style="font-size: xx-small; float: right;" runat="server" resourcekey="TranslatePlugg" /><br />
</asp:Panel>

<asp:Panel ID="pnlEditPlugg" runat="server" Visible ="false">
    <asp:HyperLink ID="hlEditPlugg" style="font-size: xx-small; float: right;" runat="server" resourcekey="EditPlugg" /><br />
</asp:Panel>

<asp:Panel ID="pnlExitTranslateMode" runat="server" Visible ="false">
    <asp:HyperLink ID="hlExitTranslateMode" style="float: right;" runat="server" resourcekey="ExitTranslateMode" /><br />
</asp:Panel>

<asp:Panel ID="pnlExitEditMode" runat="server" Visible ="false">
    <asp:HyperLink ID="hlExitEditMode" style="float: right;" runat="server" resourcekey="ExitEditMode" /><br />
</asp:Panel>

<asp:PlaceHolder ID="phComponents" runat="server"></asp:PlaceHolder>
