using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using DR.Marvin.Dashboard.Model;

namespace DR.Marvin.Dashboard.Repository.AutomapperProfiles
{
    public class LogMappingProfile : Profile
    {
        private static readonly Regex UrnDetection = new Regex(@"urn:dr:marvin:(?<type>(job)):(?<key>[^""\s&]+)");
        protected override void Configure()
        {
            CreateMap<DateTime, DateTime>().ConvertUsing<UtcTimeConverter>();
            CreateMap<DateTime?, DateTime?>().ConvertUsing<NullableUtcTimeConverter>();
            CreateMap<log, LogEntry>()
                .BeforeMap((src, dest, context) =>
                {
                    var lines = src.Message.Split('\n');
                    context.Items["Message"] = lines.FirstOrDefault();
                    context.Items["Details"] = lines.Length <= 1 ? null : lines.Skip(1).Aggregate((i, j) => $"{i}\n{j}");
                    var urnMatch = UrnDetection.Match(src.Message);
                    context.Items["Urn"] = urnMatch.Success ?urnMatch.Value : null;
                })
                .ForMember(dest => dest.LogLevel, opt => opt.MapFrom(src => (LogLevel) Enum.Parse(typeof(LogLevel), src.Level)))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.Date.ToUniversalTime()))
                .ForMember(dest => dest.MachineName, opt => opt.MapFrom(src => src.Hostname))
                .ForMember(dest => dest.ExceptionMessage, opt => opt.MapFrom(src => src.Exception))
                .ForMember(dest => dest.Message, opt => opt.ResolveUsing((src, dest, member, context) => (string) context.Items["Message"]))
                .ForMember(dest => dest.Details, opt => opt.ResolveUsing((src, dest, member, context) => (string) context.Items["Details"]))
                .ForMember(dest => dest.Appilcation, opt => opt.MapFrom(src => src.Logger))
                .ForMember(dest => dest.ExceptionStackTrace, opt => opt.Ignore())
                .ForMember(dest=>dest.Urn, opt=> opt.ResolveUsing((src, dest, member, context) => (string)context.Items["Urn"]))
                ;
        }
    }

    public class UtcTimeConverter : ITypeConverter<DateTime, DateTime>
    {
        public DateTime Convert(DateTime source, DateTime destination, ResolutionContext context)
        {
            var res =
                source.Kind == DateTimeKind.Unspecified ?
                DateTime.SpecifyKind(source, DateTimeKind.Utc) :
                (source.Kind != DateTimeKind.Utc ?
                source.ToUniversalTime() :
                source);
            return res;
        }
    }
    public class NullableUtcTimeConverter : ITypeConverter<DateTime?, DateTime?>
    {
        public DateTime? Convert(DateTime? source, DateTime? destination, ResolutionContext context)
        {

            var res =
                source?.Kind == DateTimeKind.Unspecified ?
                DateTime.SpecifyKind(source.Value, DateTimeKind.Utc) :
                (source != null && source.Value.Kind != DateTimeKind.Utc ?
                source.Value.ToUniversalTime() :
                source);
            return res;
        }
    }
}
