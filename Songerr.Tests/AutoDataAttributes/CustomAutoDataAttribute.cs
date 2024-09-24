using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Songerr.Tests.Customizations;

namespace Songerr.Tests.AutoDataAttributes;

public class CustomAutoDataAttribute() : AutoDataAttribute(() => new Fixture()
    .Customize(new AutoMoqCustomization())
    .Customize(new FixedFilePathCustomization()));