using Grpc.Core;
using GrpcUserServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GrpcUserServer.Protos.UserService;

namespace GrpcUserServer.Service
{
    public class GrpcUserService:UserServiceBase
    {
        public override async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
        {

            var a = Convert.ToInt32("ccc");

            var user = new GetUserInfoResponse
            { 
                Id = request.Id,
                Name = "张小明",
                Phone = "17621499260",
                Address = "上海市虹口区"
            };

            return await Task.FromResult(user);

        }
    }
}
