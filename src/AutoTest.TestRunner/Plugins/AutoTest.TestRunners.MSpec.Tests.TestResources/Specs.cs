using Machine.Specifications;

namespace AutoTest.TestRunners.MSpec.Tests.TestResource
{
    [Subject(typeof(object))]
    public class When_specifiying_a_context_with_subject
    {
        It should_detect_the_context;
    }

    public class When_specifiying_the_most_simple_context
    {
        It should_detect_the_context;
        int not_an_It_field;
    }
    
    class When_specifiying_a_private_context
    {
        It should_detect_the_context;
    }

    public class NoContext
    {
    }

    class When_specifiying_an_internal_context
    {
        It is_not_a_context;
    }
}