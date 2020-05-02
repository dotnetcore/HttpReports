using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Services.Language
{
    public class EnglishLanguage:ILanguage
    {

        public string Language { get; set; } = "English";

        public string StartTime { get; set; } = "StartTime";

        public string EndTime { get; set; } = "EndTime";

        public string Template_Light { get; set; } = "Light";
        public string Template_Dark { get; set; } = "Dark";


        public string Template_EditAccount { get; set; } = "Edit Account";

        public string Template_Logout { get; set; } = "Logout";


        public string Login_UserName { get; set; } = "Username";

        public string Login_Password { get; set; } = "Password";

        public string Login_Button { get; set; } = "Login";

        public string Login_UserOrPassError { get; set; } = "Wrong user name or password";

        public string Login_Success { get; set; } = "Login Success";

        public string Login_CheckRule { get; set; } = "User name or password cannot be empty";

        public string User_NotNull { get; set; } = "Cannot be empty";

        public string User_OldNewPass { get; set; } = "Old and new passwords cannot be the same";

        public string User_UpdateSuccess { get; set; } = "Success";

        public string Index_ServiceNode { get; set; } = "Service";

        public string Index_ServiceNodeTip { get; set; } = "You can click to select and deselect service";

        public string Time_15m { get; set; } = "15min";

        public string Time_30m { get; set; } = "30min";
        public string Time_1h { get; set; } = "1h";

        public string Time_4h { get; set; } = "4h";

        public string Time_12h { get; set; } = "12h";

        public string Time_24h { get; set; } = "24h";

        public string Query { get; set; } = "Query";

        public string Index_RequestCount { get; set; } = "Request count";

        public string Index_ART { get; set; } = "Average processing time"; 

        public string Index_ErrorPercent { get; set; } = "Percentage of errors";

        public string Index_APICount { get; set; } = "Total number of interfaces";

        public string Index_SelectCount { get; set; } = "Select quantity";


        public string StatusCode { get; set; } = "StatusCode";


        public string ProcessingTime { get; set; } = "processing time(ms)";

        public string ProcessingTime2 { get; set; } = "processing time";

        public string Mostrequests { get; set; } = "Most requests";

        public string RequestError { get; set; } = "Request error";

        public string Index_Fast { get; set; } = "Fast request";

        public string Index_Slow { get; set; } = "Slow request";

        public string AvgProcessingTime { get; set; } = "Average processing time";


        public string TimeNotNull { get; set; } = "Start time and end time cannot be empty";


        public string Index_OldPwd { get; set; } = "Old Password";

        public string Index_NewPwd { get; set; } = "New Password";

        public string Index_UserSave { get; set; } = "Save";


        public string User_OldPasswordError { get; set; } = "Old password error";

        public string User_AccountFormatError { get; set; } = "Username format error";

        public string User_NewPassFormatError { get; set; } = "New password format error";

        public string UpdateSuccess { get; set; } = "Success";

        public string Menu_BasicData { get; set; } = "Basic Data";

        public string Menu_TrendData { get; set; } = "Trend Data";

        public string Menu_RequestList { get; set; } = "Requests";

        public string Menu_Monitor { get; set; } = "Monitor";

        public string Trend_MinuteAvgTime { get; set; } = "Average processing time per minute (ms)";

        public string Trend_HourAvgTime { get; set; } = "Average processing time per hour (ms)";

        public string Trend_MinuteTotalCount { get; set; } = "Requests per minute";

        public string Trend_HourTotalCount { get; set; } = "Requests per hour";


        public string Trend_DayTotalCount { get; set; } = "Requests per day";

        public string Request_RequestUrl { get; set; } = "Request url";

        public string Request_IPAddress { get; set; } = "IP address";

        public string Request_NotFound { get; set; } = "Not Found";


        public string Request_BasicInfo { get; set; } = "Basic data"; 

        public string Request_Route { get; set; } = "Route";

        public string Request_Url { get; set; } = "Url";

        public string Request_Type { get; set; } = "Method";

        public string Request_Connection { get; set; } = "Connection";

        public string Request_Time { get; set; } = "Time";
        
        public string Request_StatusCode { get; set; } = "StatusCode";

        public string Request_RemoteIP { get; set; } = "Remote IP";

        public string Request_RemotePort { get; set; } = "Remote Port";
        public string Request_LocalIP { get; set; } = "Local IP";

        public string Request_LocalPort { get; set; } = "Local Port";

        public string Request_CreateTime { get; set; } = "CreateTime";

        public string Request_DetailInfo { get; set; } = "Detail data";

        public string Request_Trace { get; set; } = "Trace";

        public string Monitor_Time1Min { get; set; } = "1min";

        public string Monitor_Time3Min { get; set; } = "3min";

        public string Monitor_Time5Min { get; set; } = "5min";

        public string Monitor_Time10Min { get; set; } = "10min";

        public string Monitor_Time30Min { get; set; } = "30min";

        public string Monitor_Time1Hour { get; set; } = "1hour";

        public string Monitor_Time2Hour { get; set; } = "2hours";

        public string Monitor_Time4Hour { get; set; } = "4hours";

        public string Monitor_Time6Hour { get; set; } = "6hours";

        public string Monitor_Time8Hour { get; set; } = "8hours";

        public string Monitor_Time12Hour { get; set; } = "12hours";

        public string Monitor_Time1Day { get; set; } = "1day";


        public string Monitor_TitleNotNull { get; set; } = "Title cannot be empty";

        public string Monitor_TitleTooLong { get; set; } = "Title too long";

        public string Monitor_DescTooLong { get; set; } = "Description too long";

        public string Monitor_MustSelectNode { get; set; } = "Select at least one service node";

        public string Monitor_EmailOrWebHookNotNull { get; set; } = "Notification mailbox and push address cannot both be empty";

        public string Monitor_EmailTooLong { get; set; } = "Mailbox too long";

        public string Monitor_WebHookTooLong { get; set; } = "Push address too long";

        public string Monitor_TimeOut_TimeFormatError { get; set; } = "Response timeout monitoring - Timeout format error";

        public string Monitor_TimeOut_PercnetError { get; set; } = "Response timeout monitoring - Percentage format error";

        public string Monitor_Error_StatusCodeNotNull { get; set; } = "Request error monitoring - StatusCode cannot be empty";

        public string Monitor_Error_StatusCodeTooLong { get; set; } = "Request error monitoring - StatusCode too long";

        public string Monitor_Error_PercnetError { get; set; } = "Request error monitoring - Percentage format error";

        public string Monitor_IP_WhileListTooLong { get; set; } = "IP monitoring - White list is too long";

        public string Monitor_IP_PercentError { get; set; } = "IP monitoring - Percentage format error";

        public string Monitor_Request_FormatError { get; set; } = "Request total monitoring - Format error of maximum requests";

        public string Monitor_MustSelectType { get; set; } = "At least one monitoring type must be enabled";


        public string Monitor_ConfirmStart { get; set; } = "Are you sure you want to start the task?";


        public string Monitor_UpdateSuccess { get; set; } = "Modification succeeded";

        public string Monitor_DeleteSuccess { get; set; } = "Delete succeeded";

        public string Monitor_ConfirmStop { get; set; } = "Are you sure you want to stop the task?";

        public string Monitor_ConfirmDelete { get; set; } = "Are you sure you want to delete the task ?";

        public string Monitor_Add { get; set; } = "Add";

        public string Monitor_Title { get; set; } = "Title";

        public string Monitor_Description { get; set; } = "Description";

        public string Monitor_State { get; set; } = "State";

        public string Monitor_Frequency { get; set; } = "Frequency";

        public string Monitor_Email { get; set; } = "Email";


        public string Monitor_ServiceNode { get; set; } = "Service";

        public string Monitor_Operation { get; set; } = "Operation";

        public string Monitor_Runing { get; set; } = "Run";

        public string Monitor_Stoping { get; set; } = "Stop";

        public string Monitor_Edit { get; set; } = "Edit";

        public string Monitor_Stop { get; set; } = "Stop";

        public string Monitor_Start { get; set; } = "Run";

        public string Monitor_Delete { get; set; } = "Delete";

        public string Button_OK { get; set; } = "OK";

        public string Button_Cancel { get; set; } = "Cancel";

        public string Save_Success { get; set; } = "Saved successfully";

        public string Task { get; set; } = "Task";

        public string Monitor_Task { get; set; } = "Monitor Task";


        public string Monitor_PushUrl { get; set; } = "Push url";

        public string Monitor_Title_Placeholder { get; set; } = "Please simply enter the task title";

        public string Monitor_Desc_Placeholder { get; set; } = "Please enter task description";

        public string Monitor_Email_Placeholder { get; set; } = "Support multiple mailboxes, aa.com,bb.com,cc.com";

        public string Monitor_Push_Placeholder { get; set; } = "Alarm information push address URL";

        public string Monitor_CheckBox_Open { get; set; } = "Open";

        public string Monitor_CheckBox_Close { get; set; } = "Close";

        public string Monitor_Type_Timeout { get; set; } = "Response timeout monitoring";

        public string Monitor_Type_RequestError { get; set; } = "Request error monitoring";

        public string Monitor_Type_IP { get; set; } = "IP anomaly monitoring";

        public string Monitor_Type_RequestCount { get; set; } = "Request quantity monitoring";
         
        public string Monitor_NavTitle { get; set; } = "Monitor";

        public string Monitor_Timeout { get; set; } = "Timeout";

        public string Monitor_Timeout_Percentage { get; set; } = "Percentage";

        public string Monitor_Timeout_Description { get; set; } = "Prevent excessive request timeouts";

        public string Monitor_Timeout_Info { get; set; } = "1.Set timeout(ms)  2.Set timeout percentage, supports two decimal places，5%,0.03%,30%... , when the timeout rate reaches or exceeds the set value, an early warning will be triggered ";

        public string Monitor_HttpStatusCode { get; set; } = "HTTP status code";

        public string Monitor_HttpStatusCode_Desc { get; set; } = "Multiple separated by commas";

        public string Monitor_Type_RequestError_Desc { get; set; } = "Prevent too many errors";

        public string Monitor_Type_RequestError_Info { get; set; } = "1.Set HTTP status code,500,503,404..., multiple separated by commas  2.Set timeout percentage, supports two decimal places, 5%,0.03%,30%..., when the error rate reaches or exceeds the set value, an early warning will be triggered";

        public string Monitor_IPWhileList { get; set; } = "IP white list";

        public string Monitor_IPWhileList_Placeholder { get; set; } = "Multiple separated by commas";

        public string Monitor_IP_Desc { get; set; } = "Mainly for a large number of fixed IP requests in a period of time, like Robot request ;";

        public string Monitor_IP_Info { get; set; } = "1.Set IP white list(not required),multiple separated by commas 2.Set repetition rate,supports two decimal places,5%,0.03%,30%...,when the repetition rate reaches or exceeds the set value, an early warning will be triggered";

        public string Monitor_Max_Request { get; set; } = "Maximum number of requests";

        public string Monitor_Type_RequestCount_Desc { get; set; } = "Prevent too many requests";

        public string Monitor_Type_RequestCount_Info { get; set; } = "1.Set maximum requests,when the requested quantity reaches or exceeds the set value, an alert will be triggered";

        public string Save { get; set; } = "Save";

        public string Back { get; set; } = "Back";

        public string Warning_Title { get; set; } = "Warning trigger notice";

        public string Warning_TimeRange { get; set; } = "Time";

        public string Warning_Current { get; set; } = "Current";


        public string Warning_Threshold { get; set; } = "Threshold"; 

    }
}
