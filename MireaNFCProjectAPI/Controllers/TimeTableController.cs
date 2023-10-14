using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using RtuTc.RtuAttend.App;

namespace MireaNFCProjectAPI.Controllers
{
    //[ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {

        [HttpGet]
        public async Task<GetMeInfoResponse> Index()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var сhannel = GrpcChannel.ForAddress("https://rtu-attends.rtu-tc.ru", new GrpcChannelOptions()
            {
                HttpClient = new System.Net.Http.HttpClient()
                {
                    DefaultRequestVersion = new Version(2, 0)
                }
            });

            //var channel = GrpcChannel.ForAddress("https://rtu-attends.rtu-tc.ru");
            var client = new UserService.UserServiceClient(сhannel);
            Metadata headers = new()
            {
                { "Cookie", ".AspNetCore.Cookies=CfDJ8JhQWyuROaRDgsTOH-IX522Eq8h999eNvHdPv51Yu4lQsXi063iB5TZ8PjyL8l11f8_3oRYXpm8bUuDAL2w7rDwIZ13xVGDV2RtR8WZmFt4wYDTwpKJ4OHtwjoZgkzQwouajm-UNb1kN8913x2FAeWL96Zd7V8jFmyIt4DeRMk_60z980KV_7uvGhDY_FCSkcLVrZPx0ziZyPoywAQ2w5IDUUyyp88zJr7S0Ak3GBgFGXkhRySfVVue5jqLvLdYgoQ_XvQwX7xvR4ETOc7P-ZcrinEhuP20PZ31eQmKa8CWDFyxy3TwO4MWeAH2lEHtMzB8e5wHAFUW0MRywucypSJP48S7sfxlMXgPdoupYXG5AGC9iSMd_T3tHK737o9aIr9CXvD8fWXs1LRFG9XXmXvcBmM-oapwjaHi5KULCtBKnH3mcT7jna9ThmiBgpu3J9hVI--YBJ7krhzftLT0se1Q-J-W9Xy20LOVd6lqqxz_7yZiE4fJ048yHTYP6nnnbYU4hi4_OsEmzXZfHhz7V1tri_Mq8s9_9IPPJMyr8JvARHvOrb41TXllmgOu7BlVBtKMxCVtr9_Y7M0qpngtWn_iyYwvLm_0SlNs72a6u1Wjpj8f8YwSThWJz7Hw4q4cLc64iV_0YDhs2LttMNdbnIblKCaERJlBEGELZaxxhNgKkaS7x3aYr-GA71MaGmTBUKw_Kb4S7kZ6lOQ31F8ATB795STaHxQHPzNJRJgXfZ5HYu-9Efns2bjH7rKUckGBvCUuJavny12piv3GWRBQk_aG-Gb9RiNc9qrfPccB5REWhIyrdFwupcU_NWnHUkGmmInIBCdKAuZkBGsbNBydei-a_gSmafN-bxl3V4Ds0T3nkETU2aCBytGBW-Na0FnZAtMq5zhacflZC79O1SuAL3jCdu4AxBUp1nlG1vWRejA11iIiMme1BPLW1nybvpFNUkBIih75_ZfjrhugnwHo0-fi41pnMHFs-5kBYjAcOXL9Z; path=/; secure; samesite=lax; httponly"} };
            var responce = await client.GetMeInfoAsync(new GetMeInfoRequest(),headers);
            Console.WriteLine(responce);
            return responce;
        }
    }
}
