using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["test"].ToString());
    protected string conn = "";
    protected void Page_Load(object sender, EventArgs e)
    { }

    /* this is the method called when 'Submit' button is clicked.
     * 
     * it checks input forms and if all are filled, attempts to make
     * a query to the SQL Server for the requested tax return status.
     */
    protected void refundSubmitButton_Click(object sender, EventArgs e)
    {
        lblErrorMessage.Text = "";
        lblErrorMessage.Visible = false;

        if (socialInput.Text == "")
        {
            lblErrorMessage.Text = "Missing SSN.<br><br />";
            lblErrorMessage.Visible = true;
        }
        else if (statusInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Filing Status.<br><br />";
            lblErrorMessage.Visible = true;
        }
        else if (amtInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Refund Amount.<br><br />";
            lblErrorMessage.Visible = true;
        }
        else
        {
            conn = "SELECT     FNAME, LNAME, RSTATUS, AMT, MSTATUS, SSN FROM         [returns] WHERE     (SSN = '" + socialInput.Text + "') AND (MSTATUS = '" + statusInput.Text + "') AND (AMT = '" + amtInput.Text + "')";
            getReturn();
        }
    }

    private void getReturn()
    {
        SqlCommand cmd = new SqlCommand(conn, objConn);
        SqlDataReader rdr;
        objConn.Open();
        rdr = cmd.ExecuteReader();

        rdr.Read();
        /*        queryRsltString = String.Format("{0} {1} {2} {3} {4} {5}", rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5]);
                string[] queryStrings = queryRsltString.Split(' '); */
        rfndSSNLabel.Text = String.Format("{0}", rdr[5]);
        rfndFilingStatusLabel.Text = String.Format("{0}", rdr[4]);
        rfndAMTLabel.Text = String.Format("{0}", rdr[3]);
        rfndSTATUSLabel.Text = String.Format("{0}", rdr[2]);
        rfndLNameLabel.Text = String.Format("{0}", rdr[1]);
        rfndFNameLabel.Text = String.Format("{0}", rdr[0]);
        rdr.Close();

    }

}

