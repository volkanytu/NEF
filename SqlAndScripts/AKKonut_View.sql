SELECT
*
FROM
(
       SELECT DISTINCT
       p.ProductId AS New_KonutId
       ,q.new_taxamount AS KDVtutar
       ,q.new_bankidName AS Banka
       ,q.Name AS [Sat�� Ad] 
       ,p.new_projectidName AS Proje
       ,p.ProductNumber AS [Daire Kimlik No]
       ,p.new_blockofbuildingidName AS Ada
       ,p.new_parcelidName AS Parsel
       ,q.CustomerIdName AS M��teri
       ,p.new_unittypeidName AS [�nite Tipi]
       ,p.new_blockidName AS Blok
       ,p.new_typeofhomeidName AS [Daire Tipi]
       ,p.new_homenumber AS [Daire No]
       ,p.new_licencenumber AS [Ruhsat No]
       ,CONVERT(decimal(16, 2),p.new_netm2) AS Net
       ,CONVERT(decimal(16, 2),p.new_grossm2) AS Br�t
       ,p.new_aks AS Aks
       ,p.new_shareidName AS Payla��m
       ,CAST(q.new_salesprocessdate AS DATE) AS [Sat�� ��lem Tarihi]
       ,CAST(q.new_prepaymentdate AS DATE) AS [Kaparo ��lem Tarihi]
       ,CAST(q.new_contractdate AS DATE) AS [Sozlesme Tarihi]
       ,p.Price AS [Konut Liste Fiyat�]
       ,p.TransactionCurrencyIdName AS [Konut Liste ParaBirimi]
       ,CONVERT(decimal(16, 2),q.new_exchangerate) AS [Doviz Kuru]
       ,CONVERT(decimal(16, 2),q.TotalLineItemAmount) AS [Konut Fiyat�]
       ,q.TransactionCurrencyIdName AS [Konut Fiyat� Parabirimi]
       ,CONVERT(decimal(16, 2),ppl.Amount) AS [Konut Liste Fiyat� Yerel]
       ,CONVERT(decimal(16, 2),q.TotalAmountLessFreight) AS [�ndirimli Sat�� Fiyat�]
       ,q.TransactionCurrencyIdName AS [�ndirimli Sat�� Parabirimi]
       ,CONVERT(decimal(16, 2),q.TotalAmountLessFreight*q.new_exchangerate) AS [�ndirimli Sat�� Fiyat� Yerel]
       ,sm3.Value AS SatisState
       ,sm4.Value AS SatisStatus
       ,sm.Value AS konutstate
       ,sm2.Value AS konutstatus
       ,q.CreatedOn
       ,p.new_floornumber AS Kat
       ,DENSE_RANK() OVER (PARTITION BY p.ProductNumber ORDER BY q.CreatedOn DESC,qd.QuoteDetailId ) AS Rank
       FROM
       Product AS p (NOLOCK)
             JOIN
                    ProductPriceLevel AS ppl (NOLOCK)
                           ON
                           p.ProductId=ppl.ProductId
                           AND
                           ppl.TransactionCurrencyIdName='TL'
             LEFT JOIN
                    QuoteDetail AS qd (NOLOCK)
                           ON
                           p.ProductId=qd.ProductId
             LEFT JOIN
                    Quote AS q (NOLOCK)
                           ON
                           qd.QuoteId=q.QuoteId
                           AND
                           q.StatusCode!=6
             LEFT JOIN
                    StringMap AS sm (NOLOCK)
                           ON
                           sm.ObjectTypeCode=1024
                           AND
                           sm.AttributeName='statecode'
                           AND
                           sm.AttributeValue=p.StateCode
             LEFT JOIN
                    StringMap AS sm2 (NOLOCK)
                           ON
                           sm2.ObjectTypeCode=1024
                           AND
                           sm2.AttributeName='statuscode'
                           AND
                           sm2.AttributeValue=p.StatusCode
             LEFT JOIN
                    StringMap AS sm3 (NOLOCK)
                           ON
                           sm3.ObjectTypeCode=1084
                           AND
                           sm3.AttributeName='statecode'
                           AND
                           sm3.AttributeValue=q.StateCode
             LEFT JOIN
                    StringMap AS sm4 (NOLOCK)
                           ON
                           sm4.ObjectTypeCode=1084
                           AND
                           sm4.AttributeName='statuscode'
                           AND
                           sm4.AttributeValue=q.StatusCode
       WHERE
       p.new_projectid!='CB9CC4EF-1117-E011-817F-00123F4DA0F7'
) AS A
WHERE
A.Rank=1