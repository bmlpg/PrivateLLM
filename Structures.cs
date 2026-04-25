using OutSystems.ExternalLibraries.SDK;

namespace PrivateLLM
{
    [OSStructure(Description = "")]
    public struct Response
    {
        [OSStructureField(DataType = OSDataType.Text, IsMandatory = false)]
        public string Result;
        [OSStructureField(DataType = OSDataType.LongInteger, Description = "Duration in milliseconds.", IsMandatory = false)]
        public long Duration;
        [OSStructureField(DataType = OSDataType.Integer, IsMandatory = false)]
        public int TotalMemoryMB;
    }

    [OSStructure(Description = "")]
    public struct Request
    {
        [OSStructureField(DataType = OSDataType.Text, IsMandatory = true)]
        public string SystemPrompt;
        [OSStructureField(DataType = OSDataType.Text, IsMandatory = true)]
        public string UserPrompt;
        [OSStructureField(DataType = OSDataType.Decimal, Description = "Lower (0.0 - 0.2) for precise data extraction. Higher (0.7+) for creative summaries.\nDefault: 0.1", IsMandatory = false, DefaultValue = "0.1")]
        public float Temperature;
        [OSStructureField(DataType = OSDataType.Integer, Description = "Controls the maximum length of the response. Increase for long summaries; decrease to save processing time.\nDefault: 256", IsMandatory = false, DefaultValue = "256")]
        public int MaxTokens;
        [OSStructureField(DataType = OSDataType.Decimal, Description = "Limits the model to a 'nucleus' of high-probability words. Lower (e.g., 0.5) to make the output more focused and predictable.\nDefault: 0.9", IsMandatory = false, DefaultValue = "0.9")]
        public float TopP;
    }
}
