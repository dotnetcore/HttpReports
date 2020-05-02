using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Services.Language
{
    public class ChineseLanguage : ILanguage
    { 
        public string Language { get; set; } = "Chinese";

        public string StartTime { get; set; } = "开始时间";

        public string EndTime { get; set; } = "结束时间";

        public string Template_Light { get; set; } = "亮色模式";
        public string Template_Dark { get; set; } = "暗色模式";

        public string Template_EditAccount { get; set; } = "修改账号";

        public string Template_Logout { get; set; } = "退出登陆";


        public string Login_UserName { get; set; } = "用户名";

        public string Login_Password { get; set; } = "密码";

        public string Login_Button { get; set; } = "登 录";

        public string Login_UserOrPassError { get; set; } = "用户名或者密码错误";

        public string Login_Success { get; set; } = "登录成功";

        public string Login_CheckRule { get; set; } = "用户名或者密码不能为空";

        public string User_NotNull { get; set; } = "不能为空";

        public string User_OldNewPass { get; set; } = "新旧密码不能一样";

        public string User_UpdateSuccess { get; set; } = "修改成功";

        public string Index_ServiceNode { get; set; } = "服务节点";


        public string Index_ServiceNodeTip { get; set; } = "你可以点击选中和取消选中节点";


        public string Time_15m { get; set; } = "15分钟";

        public string Time_30m { get; set; } = "30分钟";
        public string Time_1h { get; set; } = "1小时";

        public string Time_4h { get; set; } = "4小时";

        public string Time_12h { get; set; } = "12小时";

        public string Time_24h { get; set; } = "24小时";

        public string Query { get; set; } = "查询";

        public string Index_RequestCount { get; set; } = "请求次数";

        public string Index_ART { get; set; } = "平均处理时间";

        public string Index_ErrorPercent { get; set; } = "错误率占比";

        public string Index_APICount { get; set; } = "接口总数 (已请求)";

        public string Index_SelectCount { get; set; } = "选择数量";

        public string StatusCode { get; set; } = "请求状态码";

        public string ProcessingTime { get; set; } = "请求处理时间(ms)";

        public string ProcessingTime2 { get; set; } = "处理时间";

        public string Mostrequests { get; set; } = "最多请求";

        public string RequestError { get; set; } = "请求错误";

        public string Index_Fast { get; set; } = "平均处理时间最快";

        public string Index_Slow { get; set; } = "平均处理时间最慢";


        public string AvgProcessingTime { get; set; } = "平均处理时间";

        public string TimeNotNull { get; set; } = "开始时间和结束时间不能为空";


        public string Index_OldPwd { get; set; } = "旧密码";

        public string Index_NewPwd { get; set; } = "新密码";

        public string Index_UserSave { get; set; } = "保 存";

        public string User_OldPasswordError { get; set; } = "旧密码错误";

        public string User_AccountFormatError { get; set; } = "用户名格式错误";

        public string User_NewPassFormatError { get; set; } = "新密码格式错误";

        public string UpdateSuccess { get; set; } = "修改成功";

        public string Menu_BasicData { get; set; } = "基础数据";

        public string Menu_TrendData { get; set; } = "趋势数据";

        public string Menu_RequestList { get; set; } = "请求列表";

        public string Menu_Monitor { get; set; } = "预警监控";

        public string Trend_MinuteAvgTime { get; set; } = "每分钟平均处理时间 (ms)";

        public string Trend_HourAvgTime { get; set; } = "每分钟平均处理时间 (ms)";

        public string Trend_MinuteTotalCount { get; set; } = "每分钟请求次数";

        public string Trend_HourTotalCount { get; set; } = "每小时请求次数";

        public string Trend_DayTotalCount { get; set; } = "每天请求次数";

        public string Request_RequestUrl { get; set; } = "请求地址";

        public string Request_IPAddress { get; set; } = "IP地址";

        public string Request_NotFound { get; set; } = "未知错误，没有找到相关信息";

        public string Request_BasicInfo { get; set; } = "请求基础信息"; 

        public string Request_Route { get; set; } = "请求路由";

        public string Request_Url { get; set; } = "请求地址";

        public string Request_Type { get; set; } = "请求方式";

        public string Request_Connection { get; set; } = "通信方式";

        public string Request_Time { get; set; } = "请求耗时";

        public string Request_StatusCode { get; set; } = "响应状态码";

        public string Request_RemoteIP { get; set; } = "远程IP";

        public string Request_RemotePort { get; set; } = "远程端口";
        public string Request_LocalIP { get; set; } = "本地IP";

        public string Request_LocalPort { get; set; } = "本地端口";

        public string Request_CreateTime { get; set; } = "创建时间";

        public string Request_DetailInfo { get; set; } = "请求详细信息";

        public string Request_Trace { get; set; } = "追踪";

        public string Monitor_Time1Min { get; set; } = "1分钟";

        public string Monitor_Time3Min { get; set; } = "3分钟";

        public string Monitor_Time5Min { get; set; } = "5分钟";

        public string Monitor_Time10Min { get; set; } = "10分钟";

        public string Monitor_Time30Min { get; set; } = "30分钟";

        public string Monitor_Time1Hour { get; set; } = "1小时";

        public string Monitor_Time2Hour { get; set; } = "2小时";

        public string Monitor_Time4Hour { get; set; } = "4小时";

        public string Monitor_Time6Hour { get; set; } = "6小时";

        public string Monitor_Time8Hour { get; set; } = "8小时";

        public string Monitor_Time12Hour { get; set; } = "12小时";

        public string Monitor_Time1Day { get; set; } = "1天";

        public string Monitor_TitleNotNull { get; set; } = "标题不能为空";

        public string Monitor_TitleTooLong { get; set; } = "标题过长";

        public string Monitor_DescTooLong { get; set; } = "描述过长";

        public string Monitor_MustSelectNode { get; set; } = "至少要选择一个服务节点";

        public string Monitor_EmailOrWebHookNotNull { get; set; } = "通知邮箱,推送地址不能都为空";

        public string Monitor_EmailTooLong { get; set; } = "邮箱过长";

        public string Monitor_WebHookTooLong { get; set; } = "推送地址过长";

        public string Monitor_TimeOut_TimeFormatError { get; set; } = "响应超时监控-超时时间格式错误";

        public string Monitor_TimeOut_PercnetError { get; set; } = "响应超时监控-百分比格式错误";

        public string Monitor_Error_StatusCodeNotNull { get; set; } = "请求错误监控-HTTP状态码不能为空";

        public string Monitor_Error_StatusCodeTooLong { get; set; } = "请求错误监控-HTTP状态码过长";

        public string Monitor_Error_PercnetError { get; set; } = "请求错误监控-百分比格式错误";

        public string Monitor_IP_WhileListTooLong { get; set; } = "IP监控-白名单过长";

        public string Monitor_IP_PercentError { get; set; } = "IP监控-百分比格式错误";

        public string Monitor_Request_FormatError { get; set; } = "请求量监控-最大请求数格式错误";

        public string Monitor_MustSelectType { get; set; } = "至少要开启一项监控类型";


        public string Monitor_ConfirmStart { get; set; } = "确认要开启任务吗?";


        public string Monitor_UpdateSuccess { get; set; } = "修改成功";

        public string Monitor_DeleteSuccess { get; set; } = "删除成功";

        public string Monitor_ConfirmStop { get; set; } = "确认要停止任务吗?";

        public string Monitor_ConfirmDelete { get; set; } = "确认要删除任务吗?";


        public string Monitor_Add { get; set; } = "添加";

        public string Monitor_Title { get; set; } = "标题";

        public string Monitor_Description { get; set; } = "描述";

        public string Monitor_State { get; set; } = "状态";

        public string Monitor_Frequency { get; set; } = "监控频率";

        public string Monitor_Email { get; set; } = "邮箱";


        public string Monitor_ServiceNode { get; set; } = "服务节点";

        public string Monitor_Operation { get; set; } = "操作";

        public string Monitor_Runing { get; set; } = "运行中";

        public string Monitor_Stoping { get; set; } = "已停止";

        public string Monitor_Edit { get; set; } = "编辑";

        public string Monitor_Stop { get; set; } = "停止";

        public string Monitor_Start { get; set; } = "开启";

        public string Monitor_Delete { get; set; } = "删除";

        public string Button_OK { get; set; } = "确定";

        public string Button_Cancel { get; set; } = "关闭";

        public string Save_Success { get; set; } = "保存成功";

        public string Task { get; set; } = "任务";

        public string Monitor_Task { get; set; } = "监控任务";

        public string Monitor_PushUrl { get; set; } = "推送地址";

        public string Monitor_Title_Placeholder { get; set; } = "请简单输入任务标题";

        public string Monitor_Desc_Placeholder { get; set; } = "请输入任务描述";

        public string Monitor_Email_Placeholder { get; set; } = "告警邮件接收邮箱，支持多邮箱 aa.com,bb.com,cc.com";

        public string Monitor_Push_Placeholder { get; set; } = "告警信息推送地址Url";

        public string Monitor_CheckBox_Open { get; set; } = "开";

        public string Monitor_CheckBox_Close { get; set; } = "关";

        public string Monitor_Type_Timeout { get; set; } = "响应超时监控";

        public string Monitor_Type_RequestError { get; set; } = "请求错误监控";

        public string Monitor_Type_IP { get; set; } = "IP异常监控";

        public string Monitor_Type_RequestCount { get; set; } = "请求量监控";

        public string Monitor_NavTitle { get; set; } = "预警监控";

        public string Monitor_Timeout { get; set; } = "超时时间";


        public string Monitor_Timeout_Percentage { get; set; } = "百分比";

        public string Monitor_Timeout_Description { get; set; } = "主要针对一段时间内接口调用超时率过高";

        public string Monitor_Timeout_Info { get; set; } = "1.设定接口超时时间(毫秒)；  2.设置接口超时百分比，支持 两位小数点，5%,0.03%,30%等,超时率达到或大于设定值时会触发预警；";

        public string Monitor_HttpStatusCode { get; set; } = "HTTP状态码";

        public string Monitor_HttpStatusCode_Desc { get; set; } = "多个用逗号隔开";

        public string Monitor_Type_RequestError_Desc { get; set; } = "主要针对一段时间内接口调用错误率过高";

        public string Monitor_Type_RequestError_Info { get; set; } = "1.设定Http状态码,500,503,404,多个状态码用英文逗号隔开； 2.设置命中率,支持 两位小数点，5%,0.03%,30%等，命中率达到或大于设定值时会触发预警；";

        public string Monitor_IPWhileList { get; set; } = "IP白名单";

        public string Monitor_IPWhileList_Placeholder { get; set; } = "多个用逗号隔开";

        public string Monitor_IP_Desc { get; set; } = "主要针对一段时间内大量固定IP请求，类似机器人请求;";

        public string Monitor_IP_Info { get; set; } = "1.设置IP白名单(可不填),多个IP地址中间用英文逗号分开；  2.设置重复率，支持 两位小数点，5%,0.03%,30%等,当IP重复率达到或大于设定值时,触发预警；";

        public string Monitor_Max_Request { get; set; } = "最大请求数量";

        public string Monitor_Type_RequestCount_Desc { get; set; } = "主要针对一段时间内接口大量请求";

        public string Monitor_Type_RequestCount_Info { get; set; } = "1.设置单位时间内接口最大请求量,当请求量达到或者大于设定时触发预警";

        public string Save { get; set; } = "保 存";

        public string Back { get; set; } = "返回上一页";


        public string Warning_Title { get; set; } = "预警触发通知";

        public string Warning_TimeRange { get; set; } = "时间段";

        public string Warning_Current { get; set; } = "当前值";

        public string Warning_Threshold { get; set; } = "预警值";

    }
}
