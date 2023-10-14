using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;

namespace MireaNFCProjectAPI.Controllers
{
    //[ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {
        private readonly UserService.UserServiceClient userService;
        private readonly ElderService.ElderServiceClient elderServiceClient;

        public TimeTableController(UserService.UserServiceClient userService, ElderService.ElderServiceClient elderServiceClient)
        {
            this.userService = userService;
            this.elderServiceClient = elderServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var tempClientForCookie = new HttpClient
            {
                BaseAddress = new Uri("https://rtu-attends.rtu-tc.ru")
            };
            // тестовая ручка для получения куки под пользователем b4672826-8ed3-4ad4-8ab3-3154591fa9d7

            // если нужно логиниться под другим пользователем - это можно сделать через заголовок, но сначала нужно попросить этого пользователя создать в БД
            var tempLoginResponse = await tempClientForCookie.GetAsync("debug-login");
            tempLoginResponse.EnsureSuccessStatusCode();

            Metadata headers = new()
            {
                { "Cookie", string.Join("", tempLoginResponse.Headers.GetValues(HeaderNames.SetCookie)) }
            };
            var getMeResponse = await userService.GetMeInfoAsync(new GetMeInfoRequest(), headers);

            // студент технически может быть в нескольких журналах (группа + элитка + ...)
            var availableVisitingLogs = await elderServiceClient.GetAvailableVisitingLogsAsync(new GetAvailableVisitingLogsRequest { }, headers);

            // журнал специально созданный для тестирования имеет id 7ad9b48b-9097-49d2-8d5e-4272240a677f, но можно получать через список на всякий случай
            var visitingLogId = availableVisitingLogs.Logs.First().Id;
            var visitingLogResponse = await elderServiceClient.GetSingleVisitingLogAsync(new GetSingleVisitingLogRequest
            {
                VisitingLogId = visitingLogId
            }, headers);

            // занятия раздаются по дню, далее весь пример будет привязан к выбранному тут дню
            // UTC нужен для TimeStamp
            var dateToCheck = DateTime.UtcNow;

            var lessons = await elderServiceClient.GetAvailableLessonsAsync(new GetAvailableLessonsRequest
            {
                VisitingLogId = visitingLogId,
                Date = new Google.Type.Date
                {
                    Year = dateToCheck.Year,
                    Month = dateToCheck.Month,
                    Day = dateToCheck.Day,
                },
            }, headers);

            var infoForCreateLesson = await elderServiceClient.GetDataForLessonCreationAsync(new GetDataForLessonCreationRequest
            {
                VisitingLogId = visitingLogId,
                LessonDate = dateToCheck.Date.ToTimestamp(), // занятия создаются на основании расписания, не поменяли тут Timestamp на Date 😅
            }, headers);
            CreateLessonResponse? createLessonResponse = null;
            if (lessons.Lessons.Count == 0)
            {
                // создание занятия для примера, в ответе оно не возвращается, в интерфейсе нужно будет запросить снова весь список на день
                createLessonResponse = await elderServiceClient.CreateLessonAsync(new CreateLessonRequest
                {
                    VisitingLogId = visitingLogId,
                    LessonDate = dateToCheck.Date.ToTimestamp(), // занятия создаются на основании расписания, не поменяли тут Timestamp на Date 😅
                    DisciplineId = infoForCreateLesson.Disciplines[Random.Shared.Next(infoForCreateLesson.Disciplines.Count)].Id,
                    LessonTypeId = infoForCreateLesson.LessonTypes[Random.Shared.Next(infoForCreateLesson.LessonTypes.Count)].Id,
                    TimeSlotId = infoForCreateLesson.TimeSlots[Random.Shared.Next(infoForCreateLesson.TimeSlots.Count)].TimeslotId,
                }, headers);
                // еще раз получаем список занятий, так как только что создали
                lessons = await elderServiceClient.GetAvailableLessonsAsync(new GetAvailableLessonsRequest
                {
                    VisitingLogId = visitingLogId,
                    Date = new Google.Type.Date
                    {
                        Year = dateToCheck.Year,
                        Month = DateTime.Now.Month,
                        Day = dateToCheck.Day,
                    },
                }, headers);
            }

            // берем первое занятие для подробной работы с ним
            var firstLesson = lessons.Lessons[0];

            var attendances = await elderServiceClient.GetAttendanceForLessonAsync(new GetAttendanceForLessonRequest
            {
                VisitingLogId = visitingLogId,
                LessonId = firstLesson.Id,
            }, headers);

            //var channel = GrpcChannel.ForAddress("https://rtu-attends.rtu-tc.ru");
            var client = new TimeTableService.TimeTableServiceClient(сhannel);
            Metadata headers = new()
            {
                { "Cookie", ".AspNetCore.Cookies=CfDJ8JhQWyuROaRDgsTOH-IX521h2VKCkM5rWjAhyNBTXeiX2Ya59zfw3YBZW2TS96rDc9DlCP0fb49fFtA_92V2_LdhZ2Zu6n4liqC3pcnklcfKkkUnQ3IciRuu_qFekK0k2PRjFWnUb265_wn3tW8yEobS_g0G4F7WE1oCpUw7GCT9gRZDV7r7etiIk4Zq9qgV1BJLHQTL7Ds_kTZWZV8TFnApkSpnolYMUG5Z4AAf3HA2JpMCU5Ex8-_lI5moGlHwyWi8JZZMXV3gREH4E44wSWUzncIpYCwZJubBc8VnAH04k7NHg9JwIzJ2y4cw3Y3by-OMWd9SE0m84BoOdldPNEPF0uVsh8QPYB3_nMG9g46xl0xI-Xaeybi6N_s1yo6onN6DRSpCcP9SxNk4gTgT2GSyW79XQ0GK9NA-Fo8oWxdYXTVIFqmDpuDzrpP-qHAHJwCNURQ-lOmdLwR6njcldr119psB3h7Q4YpaPq3ustQtPluYgUY2Zk0VXoy2yQjs-E15xyCCoq9-q5CBILOqYpEFoUUA03FmeczFm3NFpyTNh0NYzQZuhGJutkn5c_6aNT3I_Yxx7IpmH_cJco601Kg3gQMoem-LlSIl7fMxYPyRZI4hpk_qAsjpFG-hIHFbMIej7kaf5Lt4eOrNAiWIcrL9Ms4ZurqOdAz3eKif_z56QITYhEdVqeFf8vjEsrSde8vPbxcf3zGF7JHWXQ4n4m-EtdtKZL-3dAmd-Iok0kLPrn5R1WHD9xiL0YDCuv_TXRznnV5a90fvF4Qu4sGq5Vfk41ic1oF4w6FHb0nQ9nLC1clpN15Imb3yNkSqb0px9Q7d5Gc8ST4ibHrPoWu5tMh-l8M7uve9QVHGKBPcpINiK5oYmlgoTg6IjChQdPttMiheeRM8aDoULVovVdojWJVkaXVpi7YR1jR_4fTyXD5MPgjCzeE3DmO_JlptFQ9dNThRtIzzzdZGOtvskW3FbkhMp4N8odX_D9Itoyz1QOs3; path=/; secure; samesite=lax; httponly" }
            };
            var responce = await client.GetMeInfoAsync(new GetMeInfoRequest(),headers);
            Console.WriteLine(responce);
            return responce;
        }

        [HttpGet("/Test")]
        public string Test()
        {
            var channel = GrpcChannel.ForAddress("http://127.0.0.1:5190");
            var client = new Greeter.GreeterClient(channel);
            string responce =  client.SayHello(new HelloRequest()).Message;
            return responce;
        }
    }
}
