
using GGolbik.SecurityTools.Work;
using Microsoft.AspNetCore.Mvc;

namespace GGolbik.SecurityToolsApp.Work;


public interface IWorkerService : IWorker
{
    object? GetResult(WorkRequest request);
    object? GetResult(WorkRequest request, TimeSpan timeout);
    IActionResult GetResponse(WorkRequest request);
    IActionResult GetResponse(WorkRequest request, TimeSpan timeout);
    IList<WorkEventArgs> GetEvents();
}
