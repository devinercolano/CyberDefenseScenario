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
    /* SQL server connection object */
    protected SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["TarTS_CONN"].ToString());
    /* identification number for return */
    protected string taxID = "";
	/* queries */
    protected string q1 = "";
    protected string q2 = "";
    protected string q3 = "";

	/* this is the method called when the page is loaded
	 *
	 * our default html is fine for initial page load
	 */
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
			clearLabels();
        }
        else if (statusInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Filing Status.<br><br />";
            lblErrorMessage.Visible = true;
			clearLabels();
        }
        else if (amtInput.Text == "")
        {
            lblErrorMessage.Text = "Missing Refund Amount.<br><br />";
            lblErrorMessage.Visible = true;
			clearLabels();
        }
        else
        {
			clearLabels();
            q1 = "SELECT     TaxID, Salutation, Suffix, LastName, FirstName, SSN     FROM CitizenData     WHERE (SSN = '" + socialInput.Text + "')";
            getReturn();
        }
		return;
    }

	/* this method is called when we encounter an error in the format of the input
	 *
	 * it clears all our output labels so we don't return it to un-authenticaed users
	 */
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

	/* gets the return info from SQL Server
	 *
	 * creates remaining queries and reads output or returns error label
	 */
    private void getReturn()
    {
        SqlCommand cmd = new SqlCommand(q1, objConn);
        SqlDataReader rdr;
		bool hasrows = false;
	
        objConn.Open();
        rdr = cmd.ExecuteReader();

        /* first get labels from the CitizenData table */
        hasrows = rdr.Read();
		if ( !hasrows ) {
			clearLabels();
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

        /* now that we have taxID, we can get labels from the Resolution table */
        q2 = "SELECT     DateResolved     FROM Resolution     WHERE (TaxID = '" + taxID + "')";
        cmd = new SqlCommand(q2, objConn);
        objConn.Open();
        rdr = cmd.ExecuteReader();
        hasrows = rdr.Read();
        rfndDateResolvedLabel.Text = String.Format("<strong>Date Resolved:</strong> {0}", rdr[0]);
        rdr.Close();
		objConn.Close();

        /* we should make sure the return amount is the right type */
		float testfloat;
		if (!float.TryParse(amtInput.Text, out testfloat)) {
			clearLabels();
			lblErrorMessage.Text = "Invalid Return Amount.<br><br />";
            lblErrorMessage.Visible = true;
			return;
		}

        /* now we can get labels from the TaxReturnInformation table */
        q3 = "SELECT     DateFiled, FilingStatus, ReturnAmount     FROM TaxReturnInformation     WHERE (ReturnAmount = '" + amtInput.Text + "') AND (FilingStatus = '" + statusInput.Text + "')";
		cmd = new SqlCommand(q3, objConn);
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

