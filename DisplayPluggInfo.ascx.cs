using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using Plugghest.Base2;
using DotNetNuke.Services.Localization;

namespace Plugghest.Modules.DisplayPlugg
{
    public partial class DisplayPluggInfo : PortalModuleBase 
    {
        public int PluggId;
        public string CultureCode;
        public PluggContainer pc;
        public bool IsAuthorized;

        protected void Page_Load(object sender, EventArgs e)
        {
            PluggId = Convert.ToInt32(((DotNetNuke.Framework.CDefault)this.Page).Title);
            CultureCode = (Page as DotNetNuke.Framework.PageBase).PageCulture.Name;
            BaseHandler bh = new BaseHandler();
            pc = new PluggContainer(CultureCode, PluggId);
            IsAuthorized = ((this.UserId != -1 && pc.ThePlugg.WhoCanEdit == EWhoCanEdit.Anyone) || pc.ThePlugg.CreatedByUserId == this.UserId || (UserInfo.IsInRole("Administator")));


            if (Request.Form["__EVENTTARGET"] == "btnWhoCanEdit")
            {
                // Fire event
                btnWhoCanEdit_Click(this, new EventArgs());
            }

            if (Request.Form["__EVENTTARGET"] == "btnListed")
            {
                // Fire event
                btnListed_Click(this, new EventArgs());
            }

            UserController uc = new UserController();
            UserInfo u = uc.GetUser(PortalId, pc.ThePlugg.CreatedByUserId);
            hlCreatedBy.Text = u.DisplayName;
            hlCreatedBy.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(pc.ThePlugg.CreatedByUserId);
            lbltheCreatedOn.Text = pc.ThePlugg.CreatedOnDate.ToString();
            rblWhoCanEdit.Items.Clear();
            rblWhoCanEdit.Items.Add("Anyone");
            rblWhoCanEdit.Items.Add("Only me");
            rblWhoCanEdit.SelectedValue = "Anyone";
            switch (pc.ThePlugg.WhoCanEdit)
            {
                case EWhoCanEdit.Anyone:
                    lbltheWhoCanEdit.Text = "Anyone";
                    break;
                case EWhoCanEdit.OnlyMe:
                    lbltheWhoCanEdit.Text = "Only me";
                    rblWhoCanEdit.SelectedValue = "Only me";
                    break;
                case EWhoCanEdit.NotSet :
                    lbltheWhoCanEdit.Text = "Not set";
                    break;
            }
            rblListed.Items.Clear();
            rblListed.Items.Add(Localization.GetString("Listed.Text", this.LocalResourceFile));
            rblListed.Items.Add(Localization.GetString("NotListed.Text", this.LocalResourceFile));
            rblListed.SelectedIndex = 0;

            if (pc.ThePlugg.IsListed)
                lblTheListed.Text = "Yes";
            else
            {
                lblTheListed.Text = "No";
                rblListed.SelectedIndex = 1;
            }
            if (pc.ThePlugg.CreatedByUserId == this.UserId)
            {
                btnWhoCanEdit.Visible = true;
                btnListed.Visible = true;
            }
            pc.LoadDescription();
            if (pc.TheDescription != null)
                lbltheDespription.Text = pc.TheDescription.Text;
            else
                lbltheDespription.Text = "-";

            if(pc.ThePlugg.CreatedByUserId == this.UserId || UserInfo.IsInRole("Administator"))
            {
                btnDelete.Visible = true;
            }

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            pc.ThePlugg.IsDeleted = true;
            pc.UpdatePluggEntity();
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.HomeTabId));
        }

        protected void btnWhoCanEdit_Click(object sender, EventArgs e)
        {
            string commandArgument = !string.IsNullOrEmpty(Request.Form["__EVENTARGUMENT"])?Request.Form["__EVENTARGUMENT"]:string.Empty;
            switch (commandArgument)
            {
                case "Anyone":
                    pc.ThePlugg.WhoCanEdit = EWhoCanEdit.Anyone;
                    break;
                case "Only me":
                    pc.ThePlugg.WhoCanEdit = EWhoCanEdit.OnlyMe;
                    break;
            }
            pc.UpdatePluggEntity();
        }

        protected void btnListed_Click(object sender, EventArgs e)
        {
            if (rblListed.SelectedIndex == 0)
                pc.ThePlugg.IsListed = true;
            else
                pc.ThePlugg.IsListed = false;
            pc.UpdatePluggEntity();
        }
    }
}