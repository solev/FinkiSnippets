using FinkiSnippets.Service.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinkiSnippets.Service
{
    public interface IExportService
    {
        ExportResultsDto ExportResultsForEvent(int eventID);
        ExportOperationsDto ExportOperationsForEvent(int eventID);

        ExportResultsDto ExportResultsForEventOld(int eventID);
        ExportOperationsDto ExportOperationsForEventOld(int eventID);
    }
}
