SELECT   
	pro.new_name     
	,COUNT(*) AS [Toplam Daire]
	,ISNULL(t.Satilan,0) AS [Konut Sayýsý]
	,ISNULL(COUNT(*) - t.Satilan, COUNT(*)) AS [Satýþa Açýk Daire]
	,ISNULL(CONVERT(decimal(18, 2)
	,t.Satilan / CONVERT(decimal(18, 2)
	,COUNT(*))), 0) AS [Satýlan Daire %]
FROM
new_project AS pro (NOLOCK) 
	JOIN  
		Product AS p (NOLOCK) 
			ON 
			pro.new_projectId=p.new_projectid
	LEFT JOIN
		(
			SELECT        
				q.new_projectid AS 'Proje'
				,COUNT(*) AS 'Satilan'
			FROM
			Quote AS q (NOLOCK)
				JOIN
					QuoteDetail AS qd (NOLOCK)
						ON
						q.QuoteId=qd.QuoteId
			WHERE
			(q.new_projectid <> 'CB9CC4EF-1117-E011-817F-00123F4DA0F7') 
			AND 
			q.StatusCode!=6
			GROUP BY 
				q.new_projectid
		) AS t 
			ON 
			t.Proje = pro.new_projectId
WHERE        
	pro.new_projectId != 'CB9CC4EF-1117-E011-817F-00123F4DA0F7'
GROUP BY 
	pro.new_name, t.Satilan