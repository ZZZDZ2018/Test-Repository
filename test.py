#encoding=utf-8
import    configparser
import    csv
import    win32com.client
#import    xlrd
#import    xlwt

cf = configparser.ConfigParser()
cfname = cf.read("PythonRamup.cfg")
csv_filename = cf.get("DataInfo", "data_filename")
mdb_filename = cf.get("DBInfo","database")


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
#rs的Open（Source,ActiveConnection,CursorType,LockType,Options)

#打开文件
eFile = open('employees.csv','r')
#读取csv文件
eReader=csv.reader(eFile)
#rs.MoveFirst()
for row in eReader:
	for i in range(5):
		#print(row[i])
		#rs.AddNew()
		rs.Fields.Item(i+1).Value = row[i]
	rs.MoveNext()
	rs.Update()
eFile.close()
rs.close()

'''
def csv_to_xlsx():
    with open('employees.csv', 'r', encoding='utf-8') as f:
        read = csv.reader(f)
        workbook = xlwt.Workbook()
        sheet = workbook.add_sheet('data')# 创建一个sheet表格
        l = 0
        for line in read:
            #print(line)
            r = 0
            for i in line:
                #print(i)
                sheet.write(l, r, i)# 一个一个将单元格数据写入
                r = r + 1
            l = l + 1
        workbook.save('employees.xlsx')  # 保存Excel

if __name__ == '__main__':
	csv_to_xlsx()
'''


'''#打开文件
filename1 = 'employees.xlsx'
workbook = xlrd.open_workbook(filename1)
# 根据sheet索引或者名称获取sheet内容
sheet1 = workbook.sheet_by_index(0) # sheet索引从0开始
print(sheet1)
'''
