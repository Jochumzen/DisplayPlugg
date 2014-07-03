using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using Plugghest.Base2;

namespace Plugghest.Modules.DisplayPlugg
{
    public partial class AddNewComponentControl : PortalModuleBase
    {
        public int ComponentOrder;
        public int PluggId;
        protected void Page_Load(object sender, EventArgs e)
        {
            ddNewComponent.Items.Add(Localization.GetString("RichRichText", LocalResourceFile));
            ddNewComponent.Items.Add(Localization.GetString("RichText", LocalResourceFile));
            ddNewComponent.Items.Add(Localization.GetString("Label", LocalResourceFile));
            ddNewComponent.Items.Add(Localization.GetString("Latex", LocalResourceFile));
            ddNewComponent.Items.Add(Localization.GetString("YouTube", LocalResourceFile));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            PluggContainer pc = new PluggContainer("en-us", PluggId);
            pc.GetComponentList();
            PluggComponent cmp = new PluggComponent();
            cmp.ComponentOrder = ComponentOrder;
            cmp.ComponentType = (EComponentType)(ddNewComponent.SelectedIndex+1);
            cmp.PluggId = PluggId;
            BaseHandler bh = new BaseHandler();
            bh.AddComponent(pc, cmp);
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", "edit=0"));
        }
    }
}