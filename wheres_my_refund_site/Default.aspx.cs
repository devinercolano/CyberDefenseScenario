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
    protected SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["TarTS_CONN"].ToString());
    protected string taxID = "";
    protected string conn1 = "";
    protected string conn2 = "";
    protected string conn3 = "";
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
			return;
        }
        else if (statusInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Filing Status.<br><br />";
            lblErrorMessage.Visible = true;
			return;
        }
        else if (amtInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Refund Amount.<br><br />";
            lblErrorMessage.Visible = true;
			return;
        }
        else
        {
            conn1 = "SELECT     TaxID, Salutation, Suffix, LastName, FirstName, SSN     FROM CitizenData     WHERE (SSN = '" + socialInput.Text + "')";
            getReturn();
        }
		return;
    }

	private void clearLabels() 
	{
		rfndSSNLabel.Text = "";
		rfndFNameLabel.Text = "";
		rfndLNameLabel.Text = "";
		rfndSufxLabel.Text = "";
		rfndSalutLabel.Text = "";
		taxID = "";
		rfndDateResolvedLabel.Text = "";
		rfndAMTLabel.Text = "";
		rfndFilingStatusLabel.Text = "";
		dateFiledLabel.Text = "";
	}

    private void getReturn()
    {
        SqlCommand cmd = new SqlCommand(conn1, objConn);
        SqlDataReader rdr;
		bool hasrows = false;
        objConn.Open();
        rdr = cmd.ExecuteReader();

        /* get labels from CitizenData! */
        hasrows = rdr.Read();
		if ( !hasrows ) {
			lblErrorMessage.Text = "Invalid SSN.<br><br />";
            lblErrorMessage.Visible = true;
			return;
		}
		rfndSSNLabel.Text = String.Format("<strong>SSN:</strong> {0}", rdr[5]);
        rfndFNameLabel.Text = String.Format("{0}", rdr[4]);
        rfndLNameLabel.Text = String.Format("{0}", rdr[3]);
        rfndSufxLabel.Text = String.Format("{0}", rdr[2]);
        rfndSalutLabel.Text = String.Format("{0}", rdr[1]);
        taxID = String.Format("{0}", rdr[0]);
        rdr.Close();
		objConn.Close();

        /* now that we have taxID, we can get labels from Resolution! */
        conn2 = "SELECT     DateResolved     FROM Resolution     WHERE (TaxID = '" + taxID + "')";
        cmd = new SqlCommand(conn2, objConn);
        objConn.Open();
        rdr = cmd.ExecuteReader();
        hasrows = rdr.Read();
        rfndDateResolvedLabel.Text = String.Format("<strong>Date Resolved:</strong> {0}", rdr[0]);
        rdr.Close();
		objConn.Close();

        /* and labels from TaxReturnInformation! */
        conn3 = "SELECT     DateFiled, FilingStatus, ReturnAmount     FROM TaxReturnInformation     WHERE (ReturnAmount = '" + amtInput.Text + "') AND (FilingStatus = '" + statusInput.Text + "')";
		cmd = new SqlCommand(conn3, objConn);
        objConn.Open();
        rdr = cmd.ExecuteReader();
        hasrows = rdr.Read();
		if ( !hasrows ) {
			clearLabels();
			lblErrorMessage.Text = "Invalid Filing Status or Return Amount.<br><br />";
            lblErrorMessage.Visible = true;
			return;
		}
		rfndAMTLabel.Text = String.Format("<strong>Amount Due:</strong> ${0}", rdr[2]);
		rfndFilingStatusLabel.Text = String.Format("<strong>Filing Status:</strong> {0}", rdr[1]);
        dateFiledLabel.Text = String.Format("<strong>Date Filed:</strong> {0}", rdr[0]);
        rdr.Close();
		objConn.Close();

    }

}

