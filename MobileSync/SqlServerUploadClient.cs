using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Data.SqlClient;

namespace water3.MobileSync
{
    public sealed class SqlServerUploadClient
    {
        private readonly string _sqlServerConnectionString;

        public SqlServerUploadClient(string sqlServerConnectionString)
        {
            _sqlServerConnectionString = sqlServerConnectionString ?? throw new ArgumentNullException(nameof(sqlServerConnectionString));
        }

        public async Task<UploadBatchResultDto> SaveBatchAsync(
            UploadBatchRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Receipts.Count == 0)
                throw new InvalidOperationException("No receipt rows to upload.");

            var result = new UploadBatchResultDto();

            using (var receiptsTable = BuildReceiptsTable(request))
            using (var linesTable = BuildLinesTable(request))
            using (var con = new SqlConnection(_sqlServerConnectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = new SqlCommand("dbo.usp_MobileSync_SaveBatch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CollectorID", request.CollectorID);
                    cmd.Parameters.AddWithValue("@DeviceCode", request.DeviceCode);
                    cmd.Parameters.AddWithValue("@DeviceName", (object)request.DeviceName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeviceModel", (object)request.DeviceModel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AppVersion", (object)request.AppVersion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AutoCreateDevice", request.AutoCreateDevice);

                    var pReceipts = cmd.Parameters.AddWithValue("@Receipts", receiptsTable);
                    pReceipts.SqlDbType = SqlDbType.Structured;
                    pReceipts.TypeName = "dbo.MobileReceiptImportType";

                    var pLines = cmd.Parameters.AddWithValue("@Lines", linesTable);
                    pLines.SqlDbType = SqlDbType.Structured;
                    pLines.TypeName = "dbo.MobileReceiptImportLineType";

                    var pSyncBatchId = new SqlParameter("@SyncBatchID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pSyncBatchId);

                    var pDeviceId = new SqlParameter("@DeviceID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pDeviceId);

                    using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        // Result set 1: summary
                        if (await rd.ReadAsync(cancellationToken))
                        {
                            result.SyncBatchID = Convert.ToInt32(rd["SyncBatchID"], CultureInfo.InvariantCulture);
                            result.DeviceID = Convert.ToInt32(rd["DeviceID"], CultureInfo.InvariantCulture);
                            result.TotalRows = Convert.ToInt32(rd["TotalRows"], CultureInfo.InvariantCulture);
                            result.InsertedCount = Convert.ToInt32(rd["InsertedCount"], CultureInfo.InvariantCulture);
                            result.DuplicateCount = Convert.ToInt32(rd["DuplicateCount"], CultureInfo.InvariantCulture);
                            result.BatchStatus = Convert.ToString(rd["BatchStatus"], CultureInfo.InvariantCulture) ?? string.Empty;
                        }

                        // Result set 2: row results
                        if (await rd.NextResultAsync(cancellationToken))
                        {
                            while (await rd.ReadAsync(cancellationToken))
                            {
                                result.RowResults.Add(new UploadRowResultDto
                                {
                                    RowNo = Convert.ToInt32(rd["RowNo"], CultureInfo.InvariantCulture),
                                    LocalPaymentGuid = Convert.ToString(rd["LocalPaymentGuid"], CultureInfo.InvariantCulture) ?? string.Empty,
                                    ImportID = rd["ImportID"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["ImportID"], CultureInfo.InvariantCulture),
                                    SaveStatus = Convert.ToString(rd["SaveStatus"], CultureInfo.InvariantCulture) ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static DataTable BuildReceiptsTable(UploadBatchRequestDto request)
        {
            var table = new DataTable();
            table.Columns.Add("RowNo", typeof(int));
            table.Columns.Add("LocalPaymentGuid", typeof(string));
            table.Columns.Add("LocalReceiptNo", typeof(string));
            table.Columns.Add("SubscriberID", typeof(int));
            table.Columns.Add("PaymentDate", typeof(DateTime));
            table.Columns.Add("TotalReceived", typeof(decimal));
            table.Columns.Add("PaymentMethod", typeof(string));
            table.Columns.Add("Notes", typeof(string));

            foreach (var x in request.Receipts)
            {
                table.Rows.Add(
                    x.RowNo,
                    x.LocalPaymentGuid,
                    x.LocalReceiptNo,
                    x.SubscriberID,
                    x.PaymentDate.Date,
                    x.TotalReceived,
                    x.PaymentMethod,
                    (object)x.Notes ?? DBNull.Value);
            }

            return table;
        }

        private static DataTable BuildLinesTable(UploadBatchRequestDto request)
        {
            var table = new DataTable();
            table.Columns.Add("ReceiptRowNo", typeof(int));
            table.Columns.Add("InvoiceID", typeof(int));
            table.Columns.Add("AppliedAmount", typeof(decimal));
            table.Columns.Add("ApplicationType", typeof(string));
            table.Columns.Add("Notes", typeof(string));

            foreach (var x in request.Lines)
            {
                var row = table.NewRow();
                row["ReceiptRowNo"] = x.ReceiptRowNo;
                row["InvoiceID"] = x.InvoiceID.HasValue
                    ? (object)x.InvoiceID.Value
                    : DBNull.Value;
                row["AppliedAmount"] = x.AppliedAmount;
                row["ApplicationType"] = x.ApplicationType;
                row["Notes"] = (object)x.Notes ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}