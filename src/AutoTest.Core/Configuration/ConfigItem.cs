using System;
namespace AutoTest.Core.Configuration
{
	class ConfigItem<T>
	{
		public bool WasReadFromConfig { get; private set; }
		public bool ShouldMerge { get; private set; }
		public T Value { get; private set; }
		
		public ConfigItem(T defaultValue)
		{
			Value = defaultValue;
			WasReadFromConfig = false;
		}
		
		public ConfigItem<T> SetValue(T newValue)
		{
			Value = newValue;
			WasReadFromConfig = true;
			return this;
		}
	}
}

