/*************************************************************************
 * FeedBack Class
 *************************************************************************/

/*************************************************************************
 * Constructor ***********************************************************
 *************************************************************************/
function FeedBack
 (
  _Alias,
  _Product,
  _Deliverable,
  _ProductVersion,
  _DocumentationVersion,
  _FeedBackDivID,
  _DefaultBody
 )
{
	this.Alias                = _Alias;
	this.Product              = _Product;
	this.Deliverable	  = _Deliverable;
	this.ProductVersion       = _ProductVersion;
	this.DocumentationVersion = _DocumentationVersion;
	this.FeedBackDivID	  = _FeedBackDivID;
	this.DefaultBody 	  = _DefaultBody;
}

/*************************************************************************
 *Member Properties ******************************************************
 *************************************************************************/

//START: Button Text
FeedBack.prototype.Submit    = L_fbsend;
FeedBack.prototype.AltSubmit = L_fbaltsend;
//  END: Button Text



//CSS Class
FeedBack.prototype.table_CSS		= "fbtable";
FeedBack.prototype.tdtitle_CSS		= "fbtitletd";
FeedBack.prototype.input_CSS		= "fbinputtd";
FeedBack.prototype.textarea_CSS		= "fbtextarea";
FeedBack.prototype.verbatimtable_CSS	= "fbverbatimtable";
FeedBack.prototype.button_CSS		= "fbinputbutton";

//BTN IDs
FeedBack.prototype.YesButton_ID    = "YesButton";
FeedBack.prototype.NoButton_ID     = "NoButton";
FeedBack.prototype.BackButton_ID   = "BackButton";
FeedBack.prototype.NextButton_ID   = "NextButton";
FeedBack.prototype.SubmitButton_ID = "SubmitButton";
FeedBack.prototype.Verbatim_ID	   = "VerbatimTextArea";
FeedBack.prototype.Radio_ID	   = "fbRating";

//FeedBack Location ID's
FeedBack.prototype.SpanTag_ID = "fb";
FeedBack.prototype.DivTag_ID  = "feedbackarea";

//BTN Event Methods
FeedBack.prototype.startfeedback_EVENT      = "document.feedback.StartFeedBack([feedback])";
FeedBack.prototype.submitfeedback_EVENT     = "feedb.SubmitFeedBack()";

//Default FeedBack Values
FeedBack.prototype.Rating	   = 3; // default is 3. 3 is satisfied. 0-3 scale
FeedBack.prototype.Verbatim	   = "";
FeedBack.prototype.Title	   = document.title;
FeedBack.prototype.URL	= location.href.replace(location.hash,"");
FeedBack.prototype.SystemLanguage = navigator.systemLanguage;
FeedBack.prototype.Version	   = 2007;

/*************************************************************************
 * Member Methods ********************************************************
 *************************************************************************/
FeedBack.prototype.StartRatingsFeedBack = _StartRatingsFeedBack;
 function _StartRatingsFeedBack(FeedBackSpanTag)
{
  //build feedback div

  var stream =  '<DIV ID="feedbackarea">'
	+ '<FORM METHOD="post" ENCTYPE="text/plain" NAME="formRating">'
	+  '<H5>' + L_fb1Title_Text + '</H5>'
	+ '<P>' + L_fbintroduction + '</P>'
	+ "<table>"
	+ "<tr>"
	+ "<td>" + L_fb1Poor + "</td>"
	+ this.MakeRadio(0,"1")
	+ this.MakeRadio(0,"2")
	+ this.MakeRadio(0,"3")
	+ this.MakeRadio(0,"4")
	+ this.MakeRadio(0,"5")
	+ "<td>" + L_fb1Excellent + "</td>"
	+ "</tr>"
	+ "</table>"
	+ '<P>' + L_fb1EnterFeedbackHere_Text + '&nbsp;&nbsp;&nbsp;&nbsp;'
	+ this.MakeButton(this.SubmitButton_ID, L_fbsend, this.submitfeedback_EVENT) + '</P>'
	+ '</FORM>'
	+ '</div>';

   //load feedback div
  FeedBackSpanTag.innerHTML = stream;
 
}


 FeedBack.prototype.StartFeedBack = _StartFeedBack;
 function _StartFeedBack(FeedBackSpanTag,FeedBackSend,FeedBack,FeedBackText)

 {
  //build feedback div
    
  var subject = this.Title
  + " ("
  + "/1:"
  + this.Product
  + "/2:"
  + this.ProductVersion
  + "/3:"
  + this.DocumentationVersion
  + "/4:"
  + this.DeliverableValue()
  + "/5:"
  + this.URLValue()
  + "/6:"
  + "0"	
  + "/7:"
  + this.DeliveryType()
  + "/8:"
  + this.SystemLanguage
  + "/9:"
  + this.Version
		+ ")"; 

  var sEntireMailMessage = "MAILTO:"
  + this.Alias
  + "?subject=" + subject 
	 + "&body=" + ((this.Verbatim != "") ? this.Verbatim : this.DefaultBody);

  var stream = '<span id="feedbackarea">'
	+ FeedBackSend
	+ ' <A HREF='
	+ '"'	
	+ sEntireMailMessage
	+ '"'
	+ '>'
	+ FeedBack
	+ '</A>'
	+ FeedBackText
	+ '</span>'
	
 
  //load feedback div
  FeedBackSpanTag.innerHTML = stream;

 }
 
 FeedBack.prototype.MakeRadio = _MakeRadio;
 function _MakeRadio(val,txt)
 {

  var stream = "<td class='" + this.input_CSS + "' align=right>"
		+ txt + "<BR><input name=" + this.Radio_ID + " type=radio value=" + val + " onclick='" + this.setrating_EVENT + "' "
		+ " " + ((this.Rating == val) ? "CHECKED" : "") + ">" + "</input></td>"

  return stream;
 }
 
 FeedBack.prototype.MakeButton = _MakeButton;
 function _MakeButton(id, val, evt)
 {
 
  var stream = "<input id='submitFeedback' type=button id=" + id +" value=\"" + val + "\" onclick=\"" + evt + "\">"
	
  return stream;
 }
 
 FeedBack.prototype.SubmitFeedBack = _SubmitFeedBack;
 function _SubmitFeedBack()
 {
  var langauge;
  if(navigator.userAgent.indexOf("Firefox")!=-1)
  {
   var index = navigator.userAgent.indexOf('(');
   var string = navigator.userAgent.substring(navigator.userAgent.indexOf('('), navigator.userAgent.length);
   var splitString = string.split(';');
   language = splitString[3].substring(1, splitString[3].length);
  }
  else language = navigator.systemLanguage;

  /*if(event.srcElement.id == this.YesButton_ID)
  {
   this.Rating = 3;
   this.Verbatim = this.DefaultBody;
  }*/
  
  var subject = this.Title
  + " ("
  + "/1:"
  + this.Product
  + "/2:"
  + this.ProductVersion
  + "/3:"
  + this.DocumentationVersion
  + "/4:"
  + this.DeliverableValue()
  + "/5:"
  + this.URLValue()
  + "/6:"
  + GetRating()	//  + this.Rating
  + "/7:"
  + this.DeliveryType()
  + "/8:"
  + language
  + "/9:"
  + this.Version
		+ ")"; 

  var sEntireMailMessage = "MAILTO:"
  + this.Alias
  + "?subject=" + subject 
	 + "&body=" + ((this.Verbatim != "") ? this.Verbatim : this.DefaultBody);
	  
  location.href=sEntireMailMessage;

    
  feedb.StartRatingsFeedBack(fb);

  return;
 }
 
 FeedBack.prototype.CheckDeliverable = _CheckDeliverable;
 function _CheckDeliverable()
 {
  var stream = "CheckDeliverable";
  
 }

 FeedBack.prototype.SetRating = _SetRating;
 function _SetRating(val)
 {
  this.Rating = val;
 }

 FeedBack.prototype.ReloadFeedBack = _ReloadFeedBack;
 function _ReloadFeedBack()
 {
  location.reload(true);
 }
 
 FeedBack.prototype.ScrollToFeedBack = _ScrollToFeedBack;
 function _ScrollToFeedBack(FeedBackSpanTag)
 {
  window.scrollTo(0,10000);
  FeedBackSpanTag.scrollIntoView(true);
 }
 
 FeedBack.prototype.SetVerbatim = _SetVerbatim;
 function _SetVerbatim(TextAreaValue)
 {
  this.Verbatim = TextAreaValue;
 }
 
FeedBack.prototype.DeliveryType = _DeliveryType;
function _DeliveryType()
{
 if (this.URL.indexOf("ms-help://")!=-1) {return("h");}
	else if (this.URL.indexOf(".chm::/")!=-1) {return("c");}
	else if (this.URL.indexOf("http://")!=-1) {return("w");}
	else if (this.URL.indexOf("file:")!=-1) {return("f");}
	else return("0");
}
FeedBack.prototype.DeliverableValue = _DeliverableValue;
function _DeliverableValue()
{
 if (this.URL.indexOf("ms-help://")!=-1) 
	{
	delvalue  = location.href.slice(0,location.href.lastIndexOf("/html/"));
	delvalue  = delvalue.slice(delvalue.lastIndexOf("/")+1);
	return delvalue;
	}
	else return(this.Deliverable);
}
FeedBack.prototype.URLValue = _URLValue;
function _URLValue()
{
 if (this.URL.indexOf(".chm::")!=-1) 
	{
	a = this.URL;
	while (a.indexOf("\\") < a.indexOf(".chm::") || a.indexOf("//") > a.indexOf(".chm::")) {
		if (a.indexOf("\\")==-1)
		{
		break;
		}
		a = a.substring(a.indexOf("\\")+1,a.length);
	}
	return("ms-its:"+a)
	}
 else if (this.URL.indexOf("file:///")!=-1) 
	{
	a = this.URL;

	b = a.substring(a.lastIndexOf("html")+5,a.length);
	return("file:///"+b);
	}
	else return(this.URL);
}


//---Gets topic rating.---
function GetRating()
{

	sRating = "0";
	for(var x = 0;x < 5;x++)
  	{
		if(document.formRating.fbRating[x].checked) {sRating = x + 1;}
  	}
	return sRating;
}

function altFeedback(src) {
	src.title=L_fbaltIcon;
	return;
	}

FeedBack.prototype.HeadFeedBack = _HeadFeedBack;
function _HeadFeedBack(HeadFeedBackSpanTag, feedBack, ratings)
{
 var sstream;
 if (ratings == 'true')
 {
	sstream =  '<span ID="headfeedbackarea">'
      	+ '<a href="#Feedback" onmouseover=altFeedback(this) ID="IconFB" Target="_self">'
        + feedBack
        + '</a>'
	+ '</span>';
 }
 else
 {
   var subject = this.Title
  + " ("
  + "/1:"
  + this.Product
  + "/2:"
  + this.ProductVersion
  + "/3:"
  + this.DocumentationVersion
  + "/4:"
  + this.DeliverableValue()
  + "/5:"
  + this.URLValue()
  + "/6:"
  + "0"	
  + "/7:"
  + this.DeliveryType()
  + "/8:"
  + this.SystemLanguage
  + "/9:"
  + this.Version
		+ ")"; 

  var sEntireMailMessage = "MAILTO:"
  + this.Alias
  + "?subject=" + subject 
	 + "&body=" + ((this.Verbatim != "") ? this.Verbatim : this.DefaultBody);
   sstream =  '<span ID="headfeedbackarea">'
      	+ ' <A HREF='
	+ '"'	
	+ sEntireMailMessage
	+ '"'
	+ '>'
	+ feedBack
	+ '</A>'
	+ '</span>';
 }
   //load feedback div
   HeadFeedBackSpanTag.innerHTML = sstream;

 }

