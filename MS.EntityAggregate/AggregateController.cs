using System;
using System.Web.Http;
using MS.EntityAggregate.Dtos;

namespace MS.EntityAggregate
{
    public class AggregateController : ApiController
    {
        private readonly IEntityAggregate _entityAggregate;

        public AggregateController() : this(new EntityAggregate(EntityView.Instance)) { }

        public AggregateController(IEntityAggregate entityAggregate)
        {
            _entityAggregate = entityAggregate;
        }

        [Route("aggregate")]
        public IHttpActionResult Dispatch(EntityCommand command)
        {
            try
            {
                _entityAggregate.ProcessCommand(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}