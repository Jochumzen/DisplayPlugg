<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddNewComponentControl.ascx.cs" Inherits="Plugghest.Modules.DisplayPlugg.AddNewComponentControl" %>
<h4><asp:Label ID="lblAddComponent" runat="server" resourcekey ="AddComponent"></asp:Label></h4>
<asp:DropDownList ID="ddNewComponent" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;
<asp:Button ID="btnAdd" runat="server" resourcekey = "Add" OnClick="btnAdd_Click" />