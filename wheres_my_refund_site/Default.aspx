<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Where's my Refund?</title>
    <style type="text/css">
body {
	font-size: 20px;

}

h1 {
	margin-top: 1em;
	text-align: center;
	text-decoration: underline; 
}
p {
	font-size: 18px;
	text-align: center;
}
h2.footer {
	font-size: 15px;
	text-align: center;
}
p.footinfo {
	margin-top: 10em;
}
img.centered_banner {
	display: block;
	margin: 0 auto;
	padding: 1em;
}
p.textbox_label {
	font-size: 15px;
}
img.top_left {
	margin-top: -4.5em;
	margin-left: .5em;

}
        .style1
        {
            width: 100%;
        }
        .style2
        {
            text-align: right;
            width: 982px;
        }
        .style3
        {
            width: 1046px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <h1 align="center">
        <asp:Image ID="IRSLOGO" runat="server" AlternateText="SEAL" ImageAlign="Left" 
            ImageUrl="irs_logo.png" Width="100px" />
        Where's my Refund?</h1>
    <p>
        <asp:Image ID="DOLLARS" runat="server" Height="150px" ImageAlign="AbsMiddle" 
            ImageUrl="money-banner.jpg" style="margin-left: 0px" 
            AlternateText="MONEYSKY" />
    </p>
    <p>To view the status on your tax return refund, you must submit the following 
        information.</p>
    

    <br />
    

    <table cellspacing="3" class="style1">
        <tr>
            <td class="style2">
                SSN:</td>
            <td class="style3">
                <asp:TextBox ID="socialInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">
                Filing Status:</td>
            <td class="style3">
                <asp:TextBox ID="statusInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">
                Return Amount:</td>
            <td class="style3">
                <asp:TextBox ID="amtInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">
                </td>
            <td class="style3">
                <asp:Button ID="refundSubmitButton" runat="server" Text="Submit" 
                    onclick="refundSubmitButton_Click" />
            </td>
        </tr>
    </table>
    

    <p>
        
    

    <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
        
    </p>
    
	<p>
        <asp:Label ID="rfndSalutLabel" runat="server"></asp:Label>
        <asp:Label ID="rfndFNameLabel" runat="server"></asp:Label>
		<asp:Label ID="rfndLNameLabel" runat="server"></asp:Label>
		<asp:Label ID="rfndSufxLabel" runat="server"></asp:Label>
	</p>
	
	
    <p>

        <asp:Label ID="rfndSSNLabel" runat="server"></asp:Label>

        <asp:Label ID="rfndFilingStatusLabel" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Label ID="rfndAMTLabel" runat="server"></asp:Label>
        
        <asp:Label ID="dateFiledLabel" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Label ID="rfndDateResolvedLabel" runat="server"></asp:Label>
    </p>
    

    </form>
<p class="footinfo">
	<h2 class="footer">©2014 The Internal Revenue Service<br>The IRS is a division of the Department of Treasury.<br>ALL RIGHTS RESERVED.</h2>
</p>
</body>
</html>
