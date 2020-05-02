using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Services.Language
{
    public interface ILanguage
    {
        string Language { get; set; }

        string StartTime { get; set; } 
        string EndTime { get; set; }

        string Template_Light{ get; set; }

        string Template_Dark { get; set; }

        string Template_EditAccount { get; set; }

        string Template_Logout { get; set; }  

        string Login_UserName { get; set; }

        string Login_Password { get; set; }

        string Login_Button{ get; set; }

        string Login_UserOrPassError { get; set; }

        string Login_Success { get; set; }

        string Login_CheckRule { get; set; } 

        string User_NotNull { get; set; }

        string User_OldNewPass { get; set; }

        string User_UpdateSuccess { get; set; }

        string Index_ServiceNode { get; set; }

        string Index_ServiceNodeTip { get; set; }

        string Query { get; set; }

        string Time_15m { get; set; }

        string Time_30m { get; set; }
        string Time_1h { get; set; }

        string Time_4h { get; set; }

        string Time_12h { get; set; }

        string Time_24h { get; set; }

        string Index_RequestCount { get; set; }

        string Index_ART { get; set; }

        string Index_ErrorPercent { get; set; }

        string Index_APICount { get; set; }

        string Index_SelectCount { get; set; }

        string StatusCode { get; set; }

        string AvgProcessingTime { get; set; }

        string ProcessingTime { get; set; }

        string ProcessingTime2 { get; set; }

        string Mostrequests { get; set; }

        string RequestError { get; set; }

        string Index_Fast { get; set; }

        string Index_Slow { get; set; }

        string TimeNotNull { get; set; }

        string Index_OldPwd { get; set; }

        string Index_NewPwd { get; set; }

        string Index_UserSave { get; set; }


        string User_OldPasswordError{ get; set; }

        string User_AccountFormatError { get; set; }

        string User_NewPassFormatError { get; set; } 

        string UpdateSuccess { get; set; }

        string Menu_BasicData { get; set; }

        string Menu_TrendData{ get; set; }

        string Menu_RequestList { get; set; }

        string Menu_Monitor { get; set; }

        string Trend_MinuteAvgTime { get; set; }

        string Trend_HourAvgTime { get; set; }

        string Trend_MinuteTotalCount { get; set; }

        string Trend_HourTotalCount { get; set; }

        string Trend_DayTotalCount { get; set; }

        string Request_RequestUrl { get; set; }

        string Request_IPAddress { get; set; }

        string Request_NotFound { get; set; } 

        string Request_BasicInfo { get; set; }

        string Request_Route { get; set; }

        string Request_Url { get; set; }

        string Request_Type { get; set; }

        string Request_Connection { get; set; }

        string Request_Time { get; set; }


        string Request_StatusCode { get; set; }

        string Request_RemoteIP { get; set; }

        string Request_RemotePort { get; set; }


        string Request_LocalIP { get; set; }

        string Request_LocalPort { get; set; }

        string Request_CreateTime { get; set; }

        string Request_DetailInfo { get; set; } 

        string Request_Trace { get; set; }


        string Monitor_Time1Min { get; set; } 

        string Monitor_Time3Min { get; set; }

        string Monitor_Time5Min { get; set; } 

        string Monitor_Time10Min { get; set; }

        string Monitor_Time30Min { get; set; }

        string Monitor_Time1Hour { get; set; }

        string Monitor_Time2Hour { get; set; }

        string Monitor_Time4Hour { get; set; }

        string Monitor_Time6Hour { get; set; }

        string Monitor_Time8Hour { get; set; }

        string Monitor_Time12Hour { get; set; }

        string Monitor_Time1Day { get; set; }

        string Monitor_TitleNotNull { get; set; }

        string Monitor_TitleTooLong { get; set; }

        string Monitor_DescTooLong { get; set; }

        string Monitor_MustSelectNode { get; set; }

        string Monitor_EmailOrWebHookNotNull { get; set; }

        string Monitor_EmailTooLong { get; set; }

        string Monitor_WebHookTooLong { get; set; }

        string Monitor_TimeOut_TimeFormatError { get; set; }

        string Monitor_TimeOut_PercnetError { get; set; }

        string Monitor_Error_StatusCodeNotNull { get; set; }

        string Monitor_Error_StatusCodeTooLong { get; set; }

        string Monitor_Error_PercnetError { get; set; }

        string Monitor_IP_WhileListTooLong { get; set; }

        string Monitor_IP_PercentError { get; set; }

        string Monitor_Request_FormatError { get; set; }

        string Monitor_MustSelectType { get; set; }


        string Monitor_ConfirmStart { get; set; } 

        string Monitor_UpdateSuccess { get; set; }

        string Monitor_DeleteSuccess { get; set; }

        string Monitor_ConfirmStop { get; set; }

        string Monitor_ConfirmDelete { get; set; }

        string Monitor_Add { get; set; }

        string Monitor_Title { get; set; }


        string Monitor_Description{ get; set; }

        string Monitor_State { get; set; }

        string Monitor_Frequency { get; set; }

        string Monitor_Email { get; set; }


        string Monitor_ServiceNode { get; set; }

        string Monitor_Operation { get; set; }

        string Monitor_Runing { get; set; }

        string Monitor_Stoping { get; set; }

        string Monitor_Edit { get; set; }

        string Monitor_Stop { get; set; }

        string Monitor_Start { get; set; }

        string Monitor_Delete { get; set; }

        string Button_OK { get; set; }

        string Button_Cancel { get; set; }

        string Save_Success { get; set; }

        string Task { get; set; }

        string Monitor_Task { get; set; }

        string Monitor_PushUrl { get; set; } 

        string Monitor_Title_Placeholder { get; set; }

        string Monitor_Desc_Placeholder { get; set; }

        string Monitor_Email_Placeholder { get; set; }

        string Monitor_Push_Placeholder { get; set; }

        string Monitor_CheckBox_Open { get; set; }

        string Monitor_CheckBox_Close { get; set; }

        string Monitor_Type_Timeout { get; set; }

        string Monitor_Type_RequestError { get; set; }

        string Monitor_Type_IP { get; set; }

        string Monitor_Type_RequestCount { get; set; } 

        string Monitor_NavTitle { get; set; }

        string Monitor_Timeout { get; set; }

        string Monitor_Timeout_Percentage { get; set; }

        string Monitor_Timeout_Description { get; set; }

        string Monitor_Timeout_Info { get; set; }

        string Monitor_HttpStatusCode { get; set; }

        string Monitor_HttpStatusCode_Desc { get; set; }

        string Monitor_Type_RequestError_Desc { get; set; }

        string Monitor_Type_RequestError_Info { get; set; }

        string Monitor_IPWhileList { get; set; }

        string Monitor_IPWhileList_Placeholder { get; set; }

        string Monitor_IP_Desc { get; set; }

        string Monitor_IP_Info{ get; set; }

        string Monitor_Max_Request { get; set; }

        string Monitor_Type_RequestCount_Desc { get; set; }

        string Monitor_Type_RequestCount_Info { get; set; }

        string Save { get; set; }

        string Back { get; set; }

        string Warning_Title { get; set; }

        string Warning_TimeRange { get; set; }

        string Warning_Current { get; set; }

        string Warning_Threshold { get; set; }

    }
}
