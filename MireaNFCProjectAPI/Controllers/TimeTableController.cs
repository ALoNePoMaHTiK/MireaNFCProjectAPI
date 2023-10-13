using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcTest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RtuTc.RtuAttend.App;
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

            var updateAttendancesRequest = new UpdateAttendanceRequest
            {
                LessonId = firstLesson.Id,
            };
            // id в cookie и getme это человек, а в журнале/занятии студент - это студент, у него другая id
            var studentToUpdate = attendances.Students.First();
            updateAttendancesRequest.Records.Add(new UpdateAttendanceRecord
            {
                StudentId = studentToUpdate.StudentId,
                AttendType = studentToUpdate.ExistingAttendType == RtuTc.RtuAttend.Models.AttendType.Present 
                    ? RtuTc.RtuAttend.Models.AttendType.Absent
                    : RtuTc.RtuAttend.Models.AttendType.Present,
                // если наш студент был на паре - говорим, что не был, и наоборот. Так при каждом запросе его статус должен мигать туда обратно
            });
            // можно обновлять статусы выборочно, или все сразу. Если указаны не все - остальным проставятся RtuTc.RtuAttend.Models.AttendType.Absent, если еще небыло проставлено
            var updateAttendancesResponse = await elderServiceClient.UpdateAttendanceAsync(updateAttendancesRequest, headers);

            return Ok(new { getMeResponse, availableVisitingLogs, visitingLogResponse, lessons, infoForCreateLesson, createLessonResponse, attendances, updateAttendancesResponse });
        }

        [HttpGet("/Test")]
        public string Test()
        {
            var channel = GrpcChannel.ForAddress("http://127.0.0.1:5190");
            var client = new Greeter.GreeterClient(channel);
            string responce = client.SayHello(new HelloRequest()).Message;
            return responce;
        }
    }
}
