if jyx2_CheckEventCount(-2,-2,2) == 2 then goto labelS end;
do return end;
::labelS::
	ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	ModifyEvent(-2, 0, -2, -2, 105, -1, -1, -2, -2, -2, -2, -2, -2);
	do return end;
