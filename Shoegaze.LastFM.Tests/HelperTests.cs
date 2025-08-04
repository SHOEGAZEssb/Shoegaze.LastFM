using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoegaze.LastFM.Tests
{
  [TestFixture]
  internal class HelperTests
  {
    [Test]
    public void ParameterHelper_MakeLimitAndPageParameters_Valid_Input()
    {
      var parameters = ParameterHelper.MakeLimitAndPageParameters(limit: 10, page: 5);
      Assert.That(parameters, Has.Count.EqualTo(2));
      Assert.That(parameters, Contains.Key("limit"));
      Assert.That(parameters, Contains.Key("page"));
      Assert.Multiple(() =>
      {
        Assert.That(parameters["limit"], Is.EqualTo("10"));
        Assert.That(parameters["page"], Is.EqualTo("5"));
      });
    }

    [Test]
    public void ParameterHelper_MakeLimitAndPageParameters_Invalid_Input()
    {
      Assert.That(() => ParameterHelper.MakeLimitAndPageParameters(limit: 10, page: 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
      Assert.That(() => ParameterHelper.MakeLimitAndPageParameters(limit: 0, page: 10), Throws.InstanceOf<ArgumentOutOfRangeException>());
      Assert.That(() => ParameterHelper.MakeLimitAndPageParameters(limit: 10, page: 10), Throws.Nothing);
    }
  }
}