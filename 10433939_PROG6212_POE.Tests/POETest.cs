using Xunit;
using _10433939_PROG6212_POE.Services;
using _10433939_PROG6212_POE.Models;
using _10433939_PROG6212_POE.Data;
using System.Text;

namespace _10433939_PROG6212_POE.Tests
{
    public class POETest
    {
        [Fact]
        public void Test1_AddClaim_Successful()
        {
            //Create new
            var initialCount = ClaimData.GetAllClaims().Count();

            var newClaim = new Claim
            {
                LecturerName = "Robert Moore",
                HoursWorked = 12,
                HourlyRate = 250,
                AdditionalNotes = "No notes.",
                SubmittedBy = "Test User"
            };
            //Action
            ClaimData.AddClaim(newClaim);

            //New count
            var newCount = ClaimData.GetAllClaims().Count();
            Assert.Equal(initialCount+1, newCount);

            Assert.True(newClaim.Id > 0, "Claim should have assigned Id.");

            Assert.Equal(ClaimStatus.Pending, newClaim.Status);

            //Verify retrieval
            var retrievedClaim = ClaimData.GetClaimById(newClaim.Id);
            Assert.NotNull(retrievedClaim);
            Assert.Equal("Robert Moore", retrievedClaim.LecturerName);
        }

        [Fact]
        public async Task Test2_EncryptionFileSuccessful()
        {
            var originalContent = "This is a secret file that should be encrypted.";
            var originalBytes = Encoding.UTF8.GetBytes(originalContent);
            var inputStream = new MemoryStream(originalBytes);
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                Assert.True(File.Exists(tempFile), "Encrypted file should exist.");

                //Read
                var encryptedBytes = await File.ReadAllBytesAsync(tempFile);
                //Verify encrypted data different to original
                Assert.NotEqual(originalBytes, encryptedBytes);

                //verify encrypted file has content
                Assert.True(encryptedBytes.Length > 0, "Encrypted file should have content.");

                //Verify original text cannot be read
                var encryptedText = Encoding.UTF8.GetString(encryptedBytes);
                Assert.DoesNotContain("This is a secret file that should be encrypted.", encryptedText);
            }
            finally
            {
                if(File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
        [Fact]
        public async Task Test3_DecryptFile_Successful()
        {
            //Create and encrypt file
            var originalContent = "This is a secret document.";
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                //encrypt
                await encryptionService.EncryptFileAsync(inputStream, tempFile);
                //Decrypt
                var decryptedStream = await encryptionService.DecryptFileAsync(tempFile);
                var decryptedContent = Encoding.UTF8.GetString(decryptedStream.ToArray());

                //Verify decrypted content matched original
                Assert.Equal(originalContent, decryptedContent);
                Assert.Contains(originalContent, decryptedContent);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
        [Fact]
        public void Test4_ApproveClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "John Pork",
                HoursWorked = 12,
                HourlyRate = 250,
                AdditionalNotes = "No notes."
            };
            //Action
            ClaimData.AddClaim(newClaim);

            //Action
            var success = ClaimData.UpdateClaimStatus(newClaim.Id, ClaimStatus.Approved, "Manager", "Claim approved, good quality.");
            Assert.True(success, "Update should succeed");
            var updateClaim = ClaimData.GetClaimById(newClaim.Id);
            Assert.Equal(ClaimStatus.Approved, updateClaim.Status);
            Assert.Equal("Manager", updateClaim.ReviewedBy);
        }

        [Fact]
        public void Test5_DeclineClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "John Pork",
                HoursWorked = 12,
                HourlyRate = 250,
                AdditionalNotes = "No notes."
            };
            //Action
            ClaimData.AddClaim(newClaim);

            //Action
            var success = ClaimData.UpdateClaimStatus(newClaim.Id, ClaimStatus.Declined, "Manager", "Claim declined, not enough support from documents.");
            Assert.True(success, "Update should succeed");
            var updateClaim = ClaimData.GetClaimById(newClaim.Id);
            Assert.Equal(ClaimStatus.Declined, updateClaim.Status);
            Assert.Equal("Manager", updateClaim.ReviewedBy);
        }

        [Fact]
        public void Test6_VerifyClaim()
        {
            var newClaim = new Claim
            {
                LecturerName = "John Pork",
                HoursWorked = 12,
                HourlyRate = 250,
                AdditionalNotes = "No notes."
            };
            //Action
            ClaimData.AddClaim(newClaim);

            //Action
            var success = ClaimData.UpdateClaimStatus(newClaim.Id, ClaimStatus.Verified, "Coordinator", "Claim verified, adequate.");
            Assert.True(success, "Update should succeed");
            var updateClaim = ClaimData.GetClaimById(newClaim.Id);
            Assert.Equal(ClaimStatus.Verified, updateClaim.Status);
            Assert.Equal("Coordinator", updateClaim.ReviewedBy);
        }
    }
}
