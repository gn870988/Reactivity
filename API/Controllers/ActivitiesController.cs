using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    // 有些傳Id(例如Get、Delete)，是因為命令內部只要使用Id參數。
    // 剩下直接傳入command是因為需要用到Id(含)以外的參數。

    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List.ActivitiesEnvelope>> List(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
        {
            return await Mediator.Send(new List.Query(limit, offset, isGoing, isHost, startDate));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> Details(Guid id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            return await Mediator.Send(new Delete.Command { Id = id });
        }

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id)
        {
            return await Mediator.Send(new Attend.Command { Id = id });
        }

        [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> Unattended(Guid id)
        {
            return await Mediator.Send(new Unattended.Command { Id = id });
        }
    }
}