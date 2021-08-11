using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BasicExample.XunitExamples
{
    public class MockExamples
    {
        [Fact(DisplayName = "基本的なモック")]
        public async Task BasicMock()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 特定引数に対して固定値を返却するモック
            // モック対象: public int Add(int x, int y)
            targetMock.Setup(o => o.Add(1, 2)).Returns(3);
            // テスト
            Assert.Equal(3, target.Add(1, 2));
            Assert.Equal(0, target.Add(9, 9)); // 引数不一致の場合は既定値になる

            // 特定引数に対して固定値を返却するモック(非同期)
            // モック対象: public Task<int> AddAsync(int x, int y)
            targetMock.Setup(o => o.AddAsync(1, 2)).ReturnsAsync(3);
            // テスト
            Assert.Equal(3, await target.AddAsync(1, 2));
            Assert.Equal(0, await target.AddAsync(9, 9)); // 引数不一致の場合は既定値になる

            // 任意引数に対して固定値を返却するモック
            // モック対象: public bool Send(string message)
            targetMock.Setup(o => o.Send(It.IsAny<string>())).Returns(true);
            // テスト
            Assert.True(target.Send("abc"));
            Assert.True(target.Send("12345"));
        }

        [Fact(DisplayName = "引数に基づいた値を返却するモック")]
        public void ConditionMock()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 引数に応じた値を返却するモック(1)
            // ※Setupの条件一致が重複する場合は後勝ち
            // モック対象: public string Measure1(int str)
            targetMock.Setup(o => o.Measure1(It.IsAny<int>())).Returns("far");
            targetMock.Setup(o => o.Measure1(It.Is<int>(v => 0 < v && v < 10))).Returns("near");
            targetMock.Setup(o => o.Measure1(5)).Returns("matched");
            // テスト
            Assert.Equal("far", target.Measure1(100));
            Assert.Equal("near", target.Measure1(4));
            Assert.Equal("near", target.Measure1(6));
            Assert.Equal("matched", target.Measure1(5));

            // 引数に応じた値を返却するモック(2)
            // モック対象: public string Measure2(int str)
            targetMock
                .Setup(o => o.Measure2(It.IsAny<int>()))
                .Returns((int v) =>
                {
                    if (v == 5) return "matched";
                    else if (0 < v && v < 10) return "near";
                    else return "far";
                });
            // テスト
            Assert.Equal("far", target.Measure2(100));
            Assert.Equal("near", target.Measure2(4));
            Assert.Equal("near", target.Measure2(6));
            Assert.Equal("matched", target.Measure2(5));
        }

        [Fact(DisplayName = "例外をスローするモック")]
        public async Task ExceptionMock()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 引数なし例外をスローするモック
            // モック対象: public void Validate(string arg)
            targetMock
                .Setup(o => o.Validate("ng"))
                .Throws<NullReferenceException>();
            // テスト
            var ex1 = Assert.Throws<NullReferenceException>(() => target.Validate("ng"));
            Assert.Equal("Object reference not set to an instance of an object.", ex1.Message);

            // 引数あり例外をスローするモック
            // モック対象: public void Validate(string arg)
            targetMock
                .Setup(o => o.Validate(null))
                .Throws(new ArgumentNullException("arg"));
            // テスト
            var ex2 = Assert.Throws<ArgumentNullException>(() => target.Validate(null));
            Assert.StartsWith("Value cannot be null.", ex2.Message);

            // 引数なし例外をスローするモック(非同期)
            // モック対象: public Task ValidateAsync(string arg)
            targetMock
                .Setup(o => o.ValidateAsync("ng"))
                .Throws<NullReferenceException>();
            // テスト
            var ex3 = await Assert.ThrowsAsync<NullReferenceException>(
                async () => await target.ValidateAsync("ng"));
            Assert.StartsWith("Object reference not set to an instance of an object.", ex3.Message);
        }

        [Fact(DisplayName = "実行回数に基づいた値を返却するモック")]
        public void SequencialReturnsMock()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 実行回数に応じて異なる値を返却するモック
            // モック対象: public int Increment()
            targetMock
                .SetupSequence(o => o.Increment())
                .Returns(1)
                .Returns(2)
                .Returns(3)
                .Throws<InvalidOperationException>();
            // テスト
            Assert.Equal(1, target.Increment());
            Assert.Equal(2, target.Increment());
            Assert.Equal(3, target.Increment());
            Assert.Throws<InvalidOperationException>(() => target.Increment());
        }

        [Fact(DisplayName = "外部条件に基づいた値を返却するモック")]
        public void ExternalConditionMock()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 同一引数で異なる値を返却するモック
            // (引数以外の外部条件で返却する値を変更するモック)
            // モック対象: public string GetDate()
            string region = null;
            targetMock.When(() => region == "jp").Setup(o => o.GetDate()).Returns("1月1日");
            targetMock.When(() => region != "jp").Setup(o => o.GetDate()).Returns("1-1");
            // テスト
            region = "jp"; Assert.Equal("1月1日", target.GetDate());
            region = "en"; Assert.Equal("1-1", target.GetDate());
        }

        [Fact(DisplayName = "戻り値なしモックとその検証(1)")]
        public void NoReturnMockAndAssert()
        {
            var targetMock = new Mock<ITarget>();
            ITarget target = targetMock.Object;

            // 戻り値なしモックとその検証(1)
            // モック対象: public void TestAction1()
            targetMock.Setup(o => o.TestAction1());
            // テスト(呼び出し回数)
            targetMock.Object.TestAction1();
            targetMock.Verify(o => o.TestAction1(), Times.Once);

            // 戻り値なしモックとその検証(2)
            // モック対象: public void TestAction2(string arg)
            string innerResult = null;
            targetMock
                .Setup(o => o.TestAction2(It.IsAny<string>()))
                .Callback((string arg) => innerResult = $"arg: {arg}");
            // テスト
            targetMock.Object.TestAction2("arg1");
            Assert.Equal("arg: arg1", innerResult);
        }

        [Fact(DisplayName = "戻り値なしモックとその検証(2)")]
        public void InternalMockExample()
        {
            // モック呼び出し時の引数を保持
            var sentMessages = new List<string>();

            // テスト対象メソッドとメソッド内部で使用するモックの生成
            var mailServiceMock = new Mock<IMailService>();
            // モック対象: public bool SendMessage(string message)
            mailServiceMock
                .Setup(o => o.SendMessage(It.IsAny<string>()))
                .Callback((string message) => sentMessages.Add(message))
                .Returns(true);

            // テスト
            var messageNotifier = new MessageNotifier(mailServiceMock.Object);
            messageNotifier.SendAllMessage();
            Assert.Equal(2, sentMessages.Count);
            Assert.Equal("message1", sentMessages[0]);
            Assert.Equal("message2", sentMessages[1]);
        }

    }

    // サンプルで汎用的に使用するテスト用クラス
    public interface ITarget
    {
        public int Add(int x, int y);
        public Task<int> AddAsync(int x, int y);
        public bool Send(string message);

        public string Measure1(int str);
        public string Measure2(int str);

        public int Increment();

        public string GetDate();

        public void Validate(string arg);
        public Task ValidateAsync(string arg);

        public void TestAction1();
        public void TestAction2(string arg);
    }

    // テスト対象クラス
    // DIされたIMailServiceを使って2回メッセージを送信する。
    public class MessageNotifier
    {
        private readonly IMailService _mailService;
        public MessageNotifier(IMailService mailService) => _mailService = mailService;
        public void SendAllMessage()
        {
            _mailService.SendMessage("message1");
            _mailService.SendMessage("message2");
        }
    }
    // モック対象
    public interface IMailService
    {
        public bool SendMessage(string message);
    }
}
