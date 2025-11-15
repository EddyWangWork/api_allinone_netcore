select a.DO_TYPE,b.NIC_NAME,c.name,a.ZMONEY,* 
from INOUTCOME a
join ASSETS b on a.assetUid = b.uid
join ZCATEGORY c on a.ctgUid = c.uid
order by a.WDATE desc