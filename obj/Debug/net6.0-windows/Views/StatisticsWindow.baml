   M S B A M L   `   `   `  ����  ScheduleCreate+    %ScheduleCreate.Views.StatisticsWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$+  %Статистика загрузки��5   	   $	��600��6%   $	��800��62    � WindowStartupLocation$ CenterOwner=�5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5      �� $��Период:��6   $��Center=�6'   $��0,0,10,0q�6B   (  "System.Windows.Controls.DatePicker  5      $	��120��6<   $��0,0,10,0q�6H      SelectedDate 6   �� *	StartDate+6H   �� 5      $��-��6   $��Center=�6!   $��0,0,10,0q�6<     5      $	��120��6:    6   �� *	EndDate+6:   5      �� 5   
   $��1��6   $��0,0,0,10q�6"   .@�5      �� $ ��Преподаватели��6   .��5      &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5      ��5      �� *TeacherStatistics+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$  Преподаватель��6-   	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 200 6i   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6D   �� *Teacher.FullName+6i     5      $ Всего часов��6-   $		 100 6a   
 6B   �� *
TotalHours+6a     5      $% Количество групп��6-   $		 120 6g   
 6G   �� *TotalGroups+6g   4  .System.Windows.Controls.DataGridTemplateColumn  5      $* $Часы по дисциплинам��61   $	 * 6N      CellTemplate 5       �� 5!   "   .R�5"   &   �� ��63   �� *DisciplineHours+��5#   *   �� 5$   .   .R�5%   2   �� $ 
Horizontal=�6=   .+�5&   6   �� ��6@   �� *Key+�� 5'   6   $��: ��6@   �� 5(   6   ��6@   �� *Value+�� 5)   6   $
�� ч.��6@   5*   3   5+   /   5-   '   5.   #   50      52      53      �� 54      $��Аудитории��6   .��55        $ False��56      $
 True��57      ��55      �� *AuditoriumStatistics+ 58        59      $ Аудитория��6-   $		 100 6f   
 6@   �� *Auditorium.Number+6f     5:      $ Всего часов��6-   $		 100 6a   
 6B   �� *
TotalHours+6a     5;      $% Количество групп��6-   $		 120 6g   
 6G   �� *TotalGroups+6g     5<      $* $Часы по дисциплинам��61   $	 * 6N    5=      �� 5>   "   .R�5?   &   �� ��63   �� *DisciplineHours+��5@   *   �� 5A   .   .R�5B   2   �� $ 
Horizontal=�6=   .+�5C   6   �� ��6@   �� *Key+�� 5D   6   $��: ��6@   �� 5E   6   ��6@   �� *Value+�� 5F   6   $
�� ч.��6@   5G   3   5H   /   5J   '   5K   #   5M      5O      5P      5Q      �� 5T   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   .+�5U      �� $��Обновить��6   $	��100��6K   $��0,0,10,0q�6W   ��6(   �� *RefreshCommand+6W   �� 5V      $��Закрыть��6   $	��100��6H   ��6'   �� *CloseCommand+6H   5W      5X      5Y      