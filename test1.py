#encoding=utf-8

import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
import os
import csv
import win32com.client
import logging


def send_mail(mail_to_list,sub):
	mail_host="smtp.qq.com"
	mail_port="465"
	mail_user="646183748@qq.com"
	mail_pass="nqmbdopdqwdubeec"
	msg=MIMEMultipart()
	msg["subject"]=sub
	msg["from"]=mail_user
	msg["to"]=mail_to_list
	
	conn=win32com.client.Dispatch(r'ADODB.Connection')
	DSN='PROVIDER=Microsoft.ACE.OLEDB.12.0;DATA SOURCE=employees.accdb'
	conn.Open(DSN)
	rs=win32com.client.Dispatch(r'ADODB.Recordset')
	rs_name='employee'
	rs.Open('['+rs_name+']',conn,1,3)
	d=''
	while not rs.EOF:
		d=d+"""
		<tr>
			<td>"""+rs.Fields.Item(1).Value+"""</td>
			<td>"""+rs.Fields.Item(2).Value+"""</td>
			<td>"""+rs.Fields.Item(3).Value+"""</td>
			<td>"""+rs.Fields.Item(4).Value+"""</td>
			<td>"""+rs.Fields.Item(5).Value+"""</td>
		</tr>
			"""
		rs.MoveNext()
	html="""\
	<body>
	<div id="container">
	<p><strong>下表为员工统计信息：</strong><p>
	<div id="context">
	<table width="500" border="2" bordercolor="red" cellspacing="2">
	"""+d+"""
	</table>
	</div>
		<p><strong>以上；祝好~</strong></p>
		</div>
	<body>
	"""
	context=MIMEText(html,_subtype='html',_charset='utf-8')
	msg.attach(context)
	
	LOG_FORMAT="%(asctime)s - %(levelname)s - %(message)s"
	DATE_FORMAT="%m/%d/%y %H:%M:%S %P"
	logging.basicConfig(filename='C:/Users/admin/Desktop/test.log',filemode='a', level=logging.DEBUG, format=LOG_FORMAT, datefmt=DATE_FORMAT)
	try:
		mail = smtplib.SMTP_SSL(mail_host, mail_port)
		mail.login(mail_user, mail_pass)
		mail.sendmail(mail_user, mailto_list, msg.as_string())
		mail.quit()
		print("邮件发送成功！")
		logging.info("邮件发送成功！",exc_info=True,stack_info=True,extra={"user":"zhuzhuang"})
	except:
		mail.quit()
		print("邮件发送失败！")
		logging.info("邮件发送失败！",exc_info=True,stack_info=True,extra={"user":"zhuzhuang"})
		
if __name__ == '__main__':
	sub= "Worker Information(测试)"
	mailto_list="13260155014@163.com"
	send_mail(mailto_list,sub)