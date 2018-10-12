#encoding=utf-8
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
import os
import    csv
import    win32com.client
import  logging


def send_mail(mailto_list,sub):#to_list：收件人；sub：主题；content：邮件内容
	mail_host="smtp.qq.com"  #设置服务器
	mail_port="465"
	mail_user="646183748@qq.com"    #用户名
	mail_pass="nqmbdopdqwdubeec"   #口令
	msg = MIMEMultipart() #给定msg类型
	msg['Subject'] = sub #邮件主题
	msg['From'] = mail_user
	msg['To'] = ";".join(mailto_list)
	
	#a=os.popen("employees.accdb").read().strip('\n').split(',')
	#获取Connection对象
	conn = win32com.client.Dispatch(r'ADODB.Connection')
	#设置ConnectionString
	DSN = 'PROVIDER=Microsoft.ACE.OLEDB.12.0;DATA SOURCE=employees.accdb'
	#打开连接
	conn.Open(DSN)#这里也可以conn.Open(DSN) DSN内容和ConnectionString一致
	print("connect......")
	#打开已打开的数据库中的已有表或者视图表
	rs = win32com.client.Dispatch(r'ADODB.Recordset')
	rs_name = 'employee'
	rs.Open('[' + rs_name + ']', conn, 1, 3) #Open(sql,conn,1,3) #'3'允许更新
	d='' #表格内容
	while not rs.EOF:
		d=d+"""
		<tr>
			<td>""" + rs.Fields.Item(1).Value + """</td>
			<td>""" + rs.Fields.Item(2).Value + """</td>
			<td>""" + rs.Fields.Item(3).Value + """</td>
			<td>""" + rs.Fields.Item(4).Value + """</td>
			<td>""" + rs.Fields.Item(5).Value + """</td>
		</tr>"""
		rs.MoveNext()
	html = """\
	<body>
	<div id="container">
	<p><strong>下表为员工统计信息：</strong></p>
		<div id="content">
		<table width="500" border="2" bordercolor="red" cellspacing="2">
		"""+d+"""
		</table>
		</div>
		<p><strong>以上；祝好~</strong></p>
		</div>
	<body>
"""
	context = MIMEText(html,_subtype='html',_charset='utf-8')  #解决乱码
	msg.attach(context)
	
	LOG_FORMAT = "%(asctime)s - %(levelname)s - %(message)s"
	DATE_FORMAT = "%m/%d/%Y %H:%M:%S %p"
	logging.basicConfig(filename='C:/Users/admin/Desktop/test.log',filemode='a', level=logging.DEBUG, format=LOG_FORMAT, datefmt=DATE_FORMAT)
	
	#构造附件1
	#att1 = MIMEText(open(filename, 'rb').read(), 'xls', 'gb2312')
	#att1["Content-Type"] = 'application/octet-stream'
	#att1["Content-Disposition"] = 'attachment;filename='+filename[-6:]#这里的filename可以任意写，写什么名字，邮件中显示什么名字，filename[-6:]指的是之前附件地址的后6位
	#msg.attach(att1)
	try:
		mail = smtplib.SMTP_SSL(mail_host, mail_port)  # 使用SMTP()方法指向服务器（使用QQ邮箱服务器时，需改用 SMTP_SSL()方法）
		mail.login(mail_user, mail_pass)    # 请求服务器，登录帐号
		mail.sendmail(mail_user, mailto_list, msg.as_string())  #发送邮件
		mail.quit()     # 断开连接
		print("邮件发送成功！")
		logging.info("邮件发送成功！",exc_info=True,stack_info=True,extra={"user":"zhuzhuang"})
	except:
		mail.quit()
		print("邮件发送失败！")
		logging.info("邮件发送失败！",exc_info=True,stack_info=True,extra={"user":"zhuzhuang"})

		
if __name__ == '__main__':
	sub= "Worker Information(测试)"
	mailto_list=["v-xujie@microsoft.com","v-zhhan@microsoft.com"]
	send_mail(mailto_list,sub)