using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Should;
using TextHarbor;

namespace Tests
{
    public class ParsingTests
    {
        [Fact]
        public void parsing_single_sentence_without_dot___returns_one_block()
        {
            // Arrange
            var givenString = "This is a single sentence";
            
            // Act
            var blocks = givenString.GetBlocks();

            // Assert
            blocks.Count.ShouldEqual(1);

            var block = blocks.Single(); 
            block.Text.ShouldEqual(givenString);
            block.HarborData.BlockIndex.ShouldEqual(0);
            block.HarborData.BlockLength.ShouldEqual(givenString.Length);
        }

        [Fact]
        public void parsing_single_sentence_with_dot___returns_one_block()
        {
            // Arrange
            var givenString = "This is a single sentence with dot.";

            // Act
            var blocks = givenString.GetBlocks();

            // Assert
            blocks.Count.ShouldEqual(1);

            var block = blocks.Single();
            block.Text.ShouldEqual(givenString);
            block.HarborData.BlockIndex.ShouldEqual(0);
            block.HarborData.BlockLength.ShouldEqual(givenString.Length);
        }

        [Fact]
        public void parsing_three_sentences___returns_three_blocks()
        {
            // Arrange
            var givenString = "This is first sentence with dot. This is second. And third.";

            // Act
            var blocks = givenString.GetBlocks();

            // Assert
            blocks.Count.ShouldEqual(3);

            var firstBlock = blocks[0];
            firstBlock.Text.ShouldEqual("This is first sentence with dot.");
            firstBlock.HarborData.BlockIndex.ShouldEqual(0);
            firstBlock.HarborData.BlockLength.ShouldEqual(32);

            var secondBlock = blocks[1];
            secondBlock.Text.ShouldEqual(" This is second.");
            secondBlock.HarborData.BlockIndex.ShouldEqual(32);
            secondBlock.HarborData.BlockLength.ShouldEqual(16);

            var thirdBlock = blocks[2];
            thirdBlock.Text.ShouldEqual(" And third.");
            thirdBlock.HarborData.BlockIndex.ShouldEqual(48);
            thirdBlock.HarborData.BlockLength.ShouldEqual(11);
        }

        [Fact]
        public void parsing_sentence_and_other_text___returns_two_blocks()
        {
            // Arrange
            var givenString = "This is sentence. and that not";

            // Act
            var blocks = givenString.GetBlocks();

            // Assert
            blocks.Count.ShouldEqual(2);

            var firstBlock = blocks[0];
            firstBlock.Text.ShouldEqual("This is sentence.");
            firstBlock.HarborData.BlockIndex.ShouldEqual(0);
            firstBlock.HarborData.BlockLength.ShouldEqual(17);

            var secondBlock = blocks[1];
            secondBlock.Text.ShouldEqual(" and that not");
            secondBlock.HarborData.BlockIndex.ShouldEqual(17);
            secondBlock.HarborData.BlockLength.ShouldEqual(13);
        }
    }
}
