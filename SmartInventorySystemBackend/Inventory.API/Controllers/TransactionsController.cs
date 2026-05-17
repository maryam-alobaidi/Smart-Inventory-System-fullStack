using Inventory.BusinessLogic;
using Inventory.DataAccess;
using Inventory.DTOs.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionModel?>>> Get()
        {
            var transactions = await clsTransactions.GetAllTransactions();
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found.");
            }

            return Ok(transactions);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionModel?>> GetById(int id)
        {
            var transaction = await clsTransactions.Find(id);
            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }
            return Ok(transaction);

        }



        [HttpPost]
        public async Task<ActionResult<TransactionModel>> Add(TransactionModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            clsTransactions clstransactions = new clsTransactions();

            clstransactions.ProductID = transaction.ProductID;
            clstransactions.UserID = transaction.UserID;
            clstransactions.Type = transaction.Type;
            clstransactions.Quantity = transaction.Quantity;
            clstransactions.TransactionDate = transaction.TransactionDate;

            try
            {
                if (!await clstransactions.Save())
                {
                    return BadRequest("Failed to add new transaction.");
                }
                else
                {
                    transaction.TransactionID = clstransactions.TransactionID;
                    return CreatedAtAction(nameof(GetById), new { id = transaction.TransactionID }, transaction);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TransactionModel transaction)
        {
            if (id != transaction.TransactionID)
            {
                return BadRequest("Transaction ID mismatch.");
            }

            var existingTransaction = await clsTransactions.Find(id);
            if (existingTransaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            clsTransactions businessTransaction = new clsTransactions();


            businessTransaction.TransactionID = id;
            businessTransaction.ProductID = transaction.ProductID;
            businessTransaction.UserID = transaction.UserID;
            businessTransaction.Type = transaction.Type;
            businessTransaction.Quantity = transaction.Quantity;
            businessTransaction.TransactionDate = transaction.TransactionDate;
            businessTransaction.Mode = clsTransactions.enMode.update;



            try
            {
                if (!await businessTransaction.Save())
                {
                    return BadRequest("Failed to update transaction.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            var existingTransaction = await clsTransactions.Find(id);
            if (existingTransaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }
            try
            {
                if (!await clsTransactions.Delete(id))
                {
                    return BadRequest("Failed to delete transaction.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


    }
}