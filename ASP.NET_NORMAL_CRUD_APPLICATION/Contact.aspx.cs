using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CRUD_App
{
    public partial class Contact : System.Web.UI.Page
    {
        SqlConnection sqlConn = new SqlConnection(@"Data Source=DESKTOP-ND0CKT8\SQLEXPRESS;Initial Catalog=ASPCRUD_database;Integrated Security=true;");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                FillGridView();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearButtonEvent();
        }

        public void clearButtonEvent()
        {
            hfContactID.Value = "";
            txtName.Text = txtAddress.Text = txtContact.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            lblSuccessMessage.Text = "";
            lblErrorMessage.Text = "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (sqlConn.State == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
                btnDelete.Enabled = true;
                SqlCommand sqlcmd = new SqlCommand("ContactCreateOrUpdate", sqlConn);
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@ContactID", hfContactID.Value == "" ? 0 : Convert.ToInt32(hfContactID.Value));
                sqlcmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                sqlcmd.Parameters.AddWithValue("@Mobile", txtContact.Text.Trim());
                sqlcmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                sqlcmd.ExecuteNonQuery();
                string contactId = hfContactID.Value;
                sqlConn.Close();
                clearButtonEvent();
                if (contactId == "")
                {
                    lblSuccessMessage.Text = "Saved Successfully";
                }
                else
                {
                    lblSuccessMessage.Text = "Updated Successfully";
                }

                FillGridView();
            }

        }

        public void FillGridView()
        {
            if (sqlConn.State == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewAll", sqlConn);
                sqlDa.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                System.Data.DataTable dtbl = new System.Data.DataTable();
                sqlDa.Fill(dtbl);
                sqlConn.Close();
                gvContact.DataSource = dtbl;
                gvContact.DataBind();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (sqlConn.State == System.Data.ConnectionState.Closed)
            {

                sqlConn.Open();
                SqlCommand sqlcmd = new SqlCommand("contactDeleteByID", sqlConn);
                sqlcmd.Parameters.AddWithValue("@ContactID", Convert.ToInt32(hfContactID.Value));
                sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlcmd.ExecuteNonQuery();
                clearButtonEvent();
                sqlConn.Close();
                FillGridView();
                lblSuccessMessage.Text = "Deleted Successfully";
            }
        }

        protected void lnk_OnClick(Object sender, EventArgs e)
        {
            int contactID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (sqlConn.State == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewByID", sqlConn);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ContactID", contactID);
                sqlDa.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

                System.Data.DataTable dtbl = new System.Data.DataTable();
                sqlDa.Fill(dtbl);
                sqlConn.Close();

                hfContactID.Value = contactID.ToString();
                txtName.Text = dtbl.Rows[0]["Name"].ToString();
                txtContact.Text = dtbl.Rows[0]["Contact"].ToString();
                txtAddress.Text = dtbl.Rows[0]["Address"].ToString();

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }
    }
}