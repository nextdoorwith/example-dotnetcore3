using Moq;
using Xunit;

namespace BasicExample.XunitExamples
{
    public class MoqExamples2
    {
        [Fact(DisplayName = "モック作成比較1")]
        public void NewMockVsMockOfExample1()
        {
            var target1Mock = new Mock<ITarget>();
            ITarget target1 = target1Mock.Object;

            ITarget target2 = Mock.Of<ITarget>();

            target1.StrProp = "test";
            Assert.Null(target1.StrProp); // 既定ではプロパティ値が保持されない

            target2.StrProp = "test";
            Assert.Equal("test", target2.StrProp);
        }

        [Fact(DisplayName = "モック作成比較2")]
        public void NewMockVsMockOfExample2()
        {
            // new Mock<T>()によるプロパティ・メソッドのモック化
            var target1Mock = new Mock<ITarget>();
            target1Mock.Setup(o => o.StrProp).Returns("test");
            target1Mock.Setup(o => o.Add(2, 3)).Returns(5);
            ITarget target1 = target1Mock.Object;

            // Mock.Of<T>()によるプロパティ・メソッドのモック化(LINQ to Mocks)
            ITarget target2 = Mock.Of<ITarget>(o =>
                o.StrProp == "test" && o.Add(2, 3) == 5
            );

            // テスト
            Assert.Equal("test", target1.StrProp);
            Assert.Equal(5, target1.Add(2, 3));
            Assert.Equal("test", target2.StrProp);
            Assert.Equal(5, target2.Add(2, 3));

            // (参考)
            // モック化対象オブジェクトからモックオブジェクトの取得
            Mock<ITarget> target2Mock = Mock.Get(target2);
            target2Mock.Setup(o => o.Add(1, 2)).Returns(100);

            // テスト
            Assert.Equal("test", target2.StrProp);
            Assert.Equal(5, target2.Add(2, 3));
            Assert.Equal(100, target2.Add(1, 2));
        }

        // サンプルで汎用的に使用するテスト用クラス
        public interface ITarget
        {
            public string StrProp { get; set; }
            public int Add(int x, int y);
        }
    }
}
