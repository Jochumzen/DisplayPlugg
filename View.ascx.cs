/*
' Copyright (c) 2014  Plugghest.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Plugghest.Modules.DisplayPlugg.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Plugghest.Modules.PlugghestControls;
using Plugghest.Base2;
using System.Collections.Specialized;

namespace Plugghest.Modules.DisplayPlugg
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from DisplayPluggModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase , IActionable
    {
        public string CultureCode;
        public int PluggId;
        public PluggContainer pc;
        public bool InCreationLanguage;
        public bool IsAuthorized;
        public int Edit;
        public int Translate;
        public int Remove;
        public int DisplayInfo;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                PluggId = Convert.ToInt32(((DotNetNuke.Framework.CDefault)this.Page).Title);
                CultureCode = (Page as DotNetNuke.Framework.PageBase).PageCulture.Name;
                BaseHandler bh = new BaseHandler();
                pc = new PluggContainer(CultureCode, PluggId);
                if (pc.ThePlugg.IsDeleted)
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(Localization.GetString("TabPluggDeleted.Text", LocalResourceFile))));
                InCreationLanguage = (pc.ThePlugg.CreatedInCultureCode == CultureCode);
                IsAuthorized = ((this.UserId != -1 && pc.ThePlugg.WhoCanEdit == EWhoCanEdit.Anyone) || pc.ThePlugg.CreatedByUserId == this.UserId || (UserInfo.IsInRole("Administator")));
                Edit = !string.IsNullOrEmpty(Page.Request.QueryString["edit"]) ? Convert.ToInt16(Page.Request.QueryString["edit"]) : -1;
                Translate = !string.IsNullOrEmpty(Page.Request.QueryString["translate"]) ? Convert.ToInt16(Page.Request.QueryString["translate"]) : -1;
                Remove = !string.IsNullOrEmpty(Page.Request.QueryString["remove"]) ? Convert.ToInt16(Page.Request.QueryString["remove"]) : -1;
                DisplayInfo = !string.IsNullOrEmpty(Page.Request.QueryString["info"]) ? Convert.ToInt16(Page.Request.QueryString["info"]) : -1;

                #region hide/display controls
                hlDisplayInfo.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "info=0");
                if(DisplayInfo == 0)
                {
                    pnlDisplayInfo.Visible = false;
                    pnlHideDisplayInfo.Visible = true;
                    hlHideDisplayInfo.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "");
                }

                if (Remove > 0 && IsAuthorized)
                {
                    bh.DeleteComponent(pc, Remove);
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", "edit=0"));
                }

                if (!InCreationLanguage && UserId > -1 && Translate == -1)
                {
                    pnlToCreationLanguage.Visible = true;
                    hlToCreationLanguage.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "language=" + pc.ThePlugg.CreatedInCultureCode);
                    pnlTranslatePlugg.Visible = true;
                    hlTranslatePlugg.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "translate=0");
                }

                if (InCreationLanguage && UserId > -1 && Edit == -1)
                {
                    pnlEditPlugg.Visible = true;
                    hlEditPlugg.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "edit=0");
                }

                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                {
                    pnlExitTranslateMode.Visible = true;
                    hlExitTranslateMode.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "");
                }

                if (InCreationLanguage && UserId > -1 && Edit > -1)
                {
                    pnlExitEditMode.Visible = true;
                    hlExitEditMode.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "");
                }
                #endregion

                pc.LoadComponents();
                phComponents.Controls.Clear();
                int controlOrder = 1;
                bool editOrTranslateMode = (Edit > -1 || Translate > -1) && UserId > -1;

                if (DisplayInfo == 0)
                {
                    DisplayPluggInfo ucDPI = (DisplayPluggInfo)this.LoadControl("/DesktopModules/DisplayPlugg/DisplayPluggInfo.ascx");
                    if (ucDPI != null)
                    {
                        ucDPI.LocalResourceFile = "/DesktopModules/DisplayPlugg/App_LocalResources/DisplayPluggInfo.ascx";
                        phComponents.Controls.Add(ucDPI);
                    }
                }

                if (editOrTranslateMode)
                {
                    string ComponentHead = "<hr /><h3>" + Localization.GetString("Subject", LocalResourceFile) + "</h3>";
                    phComponents.Controls.Add(new LiteralControl(ComponentHead));
                }
                LoadSubject(controlOrder);
                controlOrder++;

                if (editOrTranslateMode)
                {
                    string ComponentHead = "<hr /><h3>" + Localization.GetString("Title", LocalResourceFile) + "</h3>";
                    phComponents.Controls.Add(new LiteralControl(ComponentHead));
                    EditTitleAndDescription(controlOrder, ETextItemType.PluggTitle);
                    controlOrder++;

                    ComponentHead = "<hr /><h3>" + Localization.GetString("Description", LocalResourceFile) + "</h3>";
                    phComponents.Controls.Add(new LiteralControl(ComponentHead));
                    EditTitleAndDescription(controlOrder, ETextItemType.PluggDescription);
                    controlOrder++;
                }

                #region AddComponents
                int componentOrder = 1;
                foreach (PluggComponent c in pc.TheComponents)
                {
                    if (Edit == 0 && IsAuthorized)
                        Add_AddNewComponentControl(componentOrder);
                    if (editOrTranslateMode)
                    {
                        string ComponentHead = "<hr /><h3>" + Localization.GetString("Component", LocalResourceFile) + " " + c.ComponentOrder + ":  " + c.ComponentType + "</h3>";
                        phComponents.Controls.Add(new LiteralControl(ComponentHead));
                    }
                    switch (c.ComponentType)
                    {
                        case EComponentType.YouTube:
                            LoadYouTube(c, controlOrder);
                            break;
                        case EComponentType.Label:
                            LoadLabel(c, controlOrder);
                            break;
                        case EComponentType.RichRichText:
                            LoadRichRich(c, controlOrder);
                            break;
                        case EComponentType.RichText:
                            LoadRich(c, controlOrder);
                            break;
                        case EComponentType.Latex:
                            LoadLatex(c, controlOrder);
                            break;
                        default:
                            break;
                    }
                    HyperLink hl = new HyperLink();
                    if (Edit == 0 && IsAuthorized)
                    {
                        hl.Text = "Remove Component";
                        hl.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "remove=" + componentOrder);
                        phComponents.Controls.Add(hl);
                    }
                    controlOrder++;
                    componentOrder++;
                }
                if (Edit == 0 && IsAuthorized)
                    Add_AddNewComponentControl(componentOrder);
                if (editOrTranslateMode)
                    phComponents.Controls.Add(new LiteralControl("<hr />"));

                #endregion
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void Add_AddNewComponentControl(int addAtPos)
        {
            phComponents.Controls.Add(new LiteralControl("<hr />"));
            AddNewComponentControl ucNC = (AddNewComponentControl)this.LoadControl("/DesktopModules/DisplayPlugg/AddNewComponentControl.ascx");
            if (ucNC != null)
            {
                ucNC.ComponentOrder = addAtPos;
                ucNC.PluggId = PluggId;
                ucNC.LocalResourceFile = "/DesktopModules/DisplayPlugg/App_LocalResources/AddNewComponentControl.ascx";
                phComponents.Controls.Add(ucNC);
            }
        }

        private void LoadSubject(int controlOrder)
        {
            SubjectControl ucS = (SubjectControl)this.LoadControl("/DesktopModules/PlugghestControls/SubjectControl.ascx");
            if (ucS != null)
            {
                ucS.SubjectCase = ESubjectCase.Plugg;
                ucS.ControlOrder = controlOrder;
                ucS.CultureCode = CultureCode;
                ucS.SubjectId = pc.ThePlugg.SubjectId;
                ucS.ItemId = PluggId;
                ucS.Case = EControlCase.View;
                if (InCreationLanguage & UserId > -1 & Edit > -1)
                    ucS.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage & IsAuthorized & Edit == controlOrder)
                    ucS.Case = EControlCase.Edit;

                ucS.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/SubjectControl.ascx";
                phComponents.Controls.Add(ucS);
            }
        }

        private void EditTitleAndDescription(int controlOrder, ETextItemType textItemType)
        {
            PureTextControl ucL = (PureTextControl)this.LoadControl("/DesktopModules/PlugghestControls/PureTextControl.ascx");
            if (ucL != null)
            {
                ucL.ModuleConfiguration = this.ModuleConfiguration;
                ucL.ItemId = PluggId;
                ucL.CultureCode = CultureCode;
                ucL.CreatedInCultureCode = pc.ThePlugg.CreatedInCultureCode;
                ucL.ControlOrder = controlOrder;
                ucL.ItemType = textItemType;
                ucL.Case = EControlCase.View;
                if (InCreationLanguage && IsAuthorized && Edit > -1)
                    ucL.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage && IsAuthorized && Edit == controlOrder)
                    ucL.Case = EControlCase.Edit;
                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                    ucL.Case = EControlCase.ViewAllowTranslate;
                if (!InCreationLanguage && UserId > -1 && Translate == controlOrder)
                    ucL.Case = EControlCase.Translate;

                ucL.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/PureTextControl.ascx";
                phComponents.Controls.Add(ucL);
            }
        }

        private void LoadYouTube(PluggComponent c, int controlOrder)
        {
            YouTubeControl ucYT = (YouTubeControl)this.LoadControl("/DesktopModules/PlugghestControls/YouTubeControl.ascx");
            if (ucYT != null)
            {
                ucYT.ModuleConfiguration = this.ModuleConfiguration;
                ucYT.PluggComponentId = c.PluggComponentId;
                ucYT.CultureCode = CultureCode;
                ucYT.ControlOrder = controlOrder;
                ucYT.Case = EControlCase.View;
                if (InCreationLanguage & IsAuthorized & Edit > -1)
                    ucYT.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage & IsAuthorized & Edit == controlOrder)
                    ucYT.Case = EControlCase.Edit;
                ucYT.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/YouTubeControl.ascx";
                phComponents.Controls.Add(ucYT);
            }
        }

        private void LoadLabel(PluggComponent c, int controlOrder)
        {
            LabelControl ucLC = (LabelControl)this.LoadControl("/DesktopModules/PlugghestControls/LabelControl.ascx");
            if (ucLC != null)
            {
                ucLC.ModuleConfiguration = this.ModuleConfiguration;
                ucLC.ItemId = c.PluggComponentId;
                ucLC.CultureCode = CultureCode;
                //ucRR.CreatedInCultureCode = pc.ThePlugg.CreatedInCultureCode;
                ucLC.ControlOrder = controlOrder;
                ucLC.ItemType = ETextItemType.PluggComponentLabel;
                ucLC.Case = EControlCase.View;
                if (InCreationLanguage & IsAuthorized & Edit > -1)
                    ucLC.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage & IsAuthorized & Edit == controlOrder)
                    ucLC.Case = EControlCase.Edit;
                ucLC.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/LabelControl.ascx";
                phComponents.Controls.Add(ucLC);
            }
        }

        private void LoadRichRich(PluggComponent c, int controlOrder)
        {
            RichRichControl ucRR = (RichRichControl)this.LoadControl("/DesktopModules/PlugghestControls/RichRichControl.ascx");
            if ((ucRR != null))
            {
                ucRR.ModuleConfiguration = this.ModuleConfiguration;
                ucRR.ItemId = c.PluggComponentId;
                ucRR.CultureCode = CultureCode;
                ucRR.CreatedInCultureCode = pc.ThePlugg.CreatedInCultureCode;
                ucRR.ControlOrder = controlOrder;
                ucRR.ItemType = ETextItemType.PluggComponentRichRichText;
                ucRR.Case = EControlCase.View;
                if (InCreationLanguage && IsAuthorized && Edit > -1)
                    ucRR.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage && IsAuthorized && Edit == controlOrder)
                    ucRR.Case = EControlCase.Edit;
                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                    ucRR.Case = EControlCase.ViewAllowTranslate;
                if (!InCreationLanguage && UserId > -1 && Translate == controlOrder)
                    ucRR.Case = EControlCase.Translate;

                ucRR.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/RichRichControl.ascx";
                phComponents.Controls.Add(ucRR);
            }
        }

        private void LoadRich(PluggComponent c, int controlOrder)
        {
            RichControl ucR = (RichControl)this.LoadControl("/DesktopModules/PlugghestControls/RichControl.ascx");
            if ((ucR != null))
            {
                ucR.ModuleConfiguration = this.ModuleConfiguration;
                ucR.ItemId = c.PluggComponentId;
                ucR.CultureCode = CultureCode;
                ucR.CreatedInCultureCode = pc.ThePlugg.CreatedInCultureCode;
                ucR.ControlOrder = controlOrder;
                ucR.ItemType = ETextItemType.PluggComponentRichText;
                ucR.Case = EControlCase.View;
                if (InCreationLanguage && IsAuthorized && Edit > -1)
                    ucR.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage && IsAuthorized && Edit == controlOrder)
                    ucR.Case = EControlCase.Edit;
                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                    ucR.Case = EControlCase.ViewAllowTranslate;
                if (!InCreationLanguage && UserId > -1 && Translate == controlOrder)
                    ucR.Case = EControlCase.Translate;

                ucR.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/RichControl.ascx";
                phComponents.Controls.Add(ucR);
            }
        }

        private void LoadLatex(PluggComponent c, int controlOrder)
        {
            LatexControl ucL = (LatexControl)this.LoadControl("/DesktopModules/PlugghestControls/LatexControl.ascx");
            if ((ucL != null))
            {
                ucL.ModuleConfiguration = this.ModuleConfiguration;
                ucL.ItemId = c.PluggComponentId;
                ucL.CultureCode = CultureCode;
                ucL.CreatedInCultureCode = pc.ThePlugg.CreatedInCultureCode;
                ucL.ControlOrder = controlOrder;
                ucL.ItemType = ELatexItemType.PluggComponentLatex;
                ucL.Case = EControlCase.View;
                if (InCreationLanguage && IsAuthorized && Edit > -1)
                    ucL.Case = EControlCase.ViewAllowEdit;
                if (InCreationLanguage && IsAuthorized && Edit == controlOrder)
                    ucL.Case = EControlCase.Edit;
                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                    ucL.Case = EControlCase.ViewAllowTranslate;
                if (!InCreationLanguage && UserId > -1 && Translate == controlOrder)
                    ucL.Case = EControlCase.Translate;

                ucL.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/LatexControl.ascx";
                phComponents.Controls.Add(ucL);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }
    }
}