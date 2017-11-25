#IncludeFile .\Init.inc

*Navigate Unbound Column Object
*Action Unbound column
*FillForm
 Column Name = unboundcolumn
 Unbound Type = Decimal
 Display Format = {0:C}
 Format Type = Numeric
 Expression = Pay
*Action OK
*Action New
*FillForm
 Pay = 1.2
*Action Save and Close
*CheckTable
 Columns = unboundcolumn
 Row[0] = $1.20

