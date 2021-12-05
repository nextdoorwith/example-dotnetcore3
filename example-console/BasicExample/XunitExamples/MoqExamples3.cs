using Moq;
using Xunit;

namespace BasicExample.XunitExamples
{
    public class MoqExamples3
    {
        [Fact(DisplayName = "プロパティ操作の検証")]
        public void AssertProperty()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            targetMock.SetupProperty(o => o.StrProp);

            target.StrProp = "test";
            Assert.Equal("test", target.StrProp);
        }

        [Fact(DisplayName = "プロパティ操作の一括検証")]
        public void VerifyPropertiesAtOnce()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 一括検証の対象とするためにVerifiable()を指定
            targetMock
                .SetupGet(o => o.StrProp)
                .Verifiable();
            targetMock
                .SetupSet(o => o.IntProp = 100) // 検証時の期待値(初期値設定ではない！)
                .Verifiable();

            _ = target.StrProp;
            target.IntProp = 1;
            target.IntProp = 100;
            target.IntProp = 100;
            target.IntProp = 200;

            targetMock.Verify(); // 一括検証(検証対象を纏めて検証)

            // 値を保持していないので、最終的な値を検証できない。
            //Assert.Equal(200, target.IntProp); // NG
            Assert.Equal(0, target.IntProp);
        }

        [Fact(DisplayName = "プロパティ操作の個別検証")]
        public void VerifyProperties()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            _ = target.StrProp;
            target.IntProp = 1;
            target.IntProp = 100;
            target.IntProp = 100;
            target.IntProp = 200;

            targetMock.VerifyGet(o => o.StrProp); // 1回のみ(既定)
            targetMock.VerifySet(o => o.IntProp = 100, Times.Exactly(2)); // 2回
            targetMock.VerifySet(o => o.IntProp = 300, Times.Never); // 0回

            // 値を保持していないので、最終的な値を検証できない。
            //Assert.Equal(200, target.IntProp); // NG
            Assert.Equal(0, target.IntProp);
        }

        [Fact(DisplayName = "VerifySet()の検証動作")]
        public void VerifySetVariation()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            target.IntProp = 100;
            target.IntProp = 101;
            target.IntProp = 101;
            target.IntProp = 102;
            target.IntProp = 102;
            target.IntProp = 102; // 最終値

            // プロパティを個別に検証
            targetMock.VerifySet(o => o.IntProp = 100);                    // 1回のみ
            targetMock.VerifySet(o => o.IntProp = 100, Times.Once);        // 1回のみ
            targetMock.VerifySet(o => o.IntProp = 101, Times.AtLeastOnce); // 1回以上
            targetMock.VerifySet(o => o.IntProp = 102, Times.AtLeast(2));  // 2回以上
            targetMock.VerifySet(o => o.IntProp = 102, Times.Exactly(3));  // 3回
            targetMock.VerifySet(o => o.IntProp = 999, Times.Never);       // 0回

            // 値を保持していないので、最終的な値を検証できない。
            //Assert.Equal(102, target.IntProp); // NG
            Assert.Equal(0, target.IntProp);
        }

        // サンプルで汎用的に使用するテスト用クラス
        public interface ITarget
        {
            public string StrProp { get; set; }
            public string ReadonlyStrProp { get; }
            public int IntProp { get; set; }
            public int ReadonlyIntProp { get; }
            public bool BoolProp { get; set; }
            public bool ReadonlyBoolProp { get; }
        }
    }
}
