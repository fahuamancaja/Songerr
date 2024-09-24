using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Songerr.Tests.Customizations;

namespace Songerr.Tests.AutoDataAttributes;

public class CustomAutoDataAttribute : AutoDataAttribute
{
    public CustomAutoDataAttribute()
        : base(() => new Fixture()
            .Customize(new AutoMoqCustomization())
            .Customize(new FixedFilePathCustomization()))
    {
    }
}