c      This program changes a *.bdf file to a FMMBEM3D input file.
c      It is written by Shinzo UChiyama. 2007.4.17
c
c
c
c

       program bdf2fmm
       implicit none

c      a text line of the *.bdf file.        
       character bdf_line*75
       integer elem_no , shee_no , grid_no1 , grid_no2 , grid_no3
       integer grid_no 
       real*8  grid_c1 , grid_c2 , grid_c3
       integer flag_out
       integer c2_bmin_cnt , c2_bmax_cnt , c3_bmin_cnt 
       integer c3_bmax_cnt
       integer bound_num   , elem_nv(2,100)
       real*8 c2_min , c2_max , c3_min , c3_max , c_error
       character bdf_file*75 , buf_line*75
       real*8  c2_bmin(3,1000) , c2_bmax(3,1000)
       real*8  c3_bmin(3,1000) , c3_bmax(3,1000)
       integer c2_bmin_no(1000), c2_bmax_no(1000)
       integer c3_bmin_no(1000), c3_bmax_no(1000)
       integer i,j,k,l
       integer count_grid , count_elem
       integer count_grid_chk , count_elem_chk
       integer layer_num , cp_no(4,20)
       real    omega,cycle_l,del_omega_c2,del_omega_c3
       integer layer_in,layer_out
       real    eps(2,20),myu(2,20)
       integer cp_cnt1,cp_cnt2,cp_cnt3,cp_cnt4

c      conditions
c      number of grids on the boundary
       c2_bmin_cnt = 0
       c2_bmax_cnt = 0
       c3_bmin_cnt = 0
       c3_bmax_cnt = 0
       cp_cnt1 = 0
       cp_cnt2 = 0
       cp_cnt3 = 0
       cp_cnt4 = 0
       count_grid  = 0
       count_elem  = 0
       count_grid_chk = 0
       count_elem_chk = 0

c      Reading conditions from the file     ./a.out < input
       write(*,*) 'HOW TO USE:exefile < input'
       read(*,'(a)') bdf_file
c       write(*,*) 'output file            :' // bdf_file
       read(*,*) c2_min
c       write(*,*) 'minimam c2 cordination :',c2_min
       read(*,*) c2_max
c       write(*,*) 'maximam c2 cordination :',c2_max
       read(*,*) c3_min
c       write(*,*) 'minimam c3 cordination :',c3_min
       read(*,*) c3_max
c       write(*,*) 'maximam c3 cordination :',c3_max
       read(*,*) c_error
c       write(*,*) 'cordination diff. between both sides:',c_error
       read(*,*) layer_num
       read(*,*) bound_num

c       read(*,*) buf_line
       do i=1 , bound_num
          read(*,*) j , elem_nv(1,j) , elem_nv(2,j)
       end do


       
c      Opening the bdf file
       open(91,FILE=bdf_file)

       do
         read(91,'(A72)',end=9101) bdf_line
         flag_out = 0

c        Reading cordinates of the grid
         if ( bdf_line(1:5) .eq. 'GRID ' ) then
c        Transration from the character value to the numerical value
            read( bdf_line(10:24) , * ) grid_no
            read( bdf_line(25:32) , * ) grid_c1
            read( bdf_line(33:40) , * ) grid_c2
            read( bdf_line(41:48) , * ) grid_c3
            flag_out = 1

         else if (bdf_line(1:5) .eq. 'GRID*' ) then
            read( bdf_line(10:24) , * ) grid_no
            read( bdf_line(41:56) , * ) grid_c1
            read( bdf_line(57:72) , * ) grid_c2

            read(91,'(a72)',end=9101) bdf_line

            read( bdf_line(9:24)  , * ) grid_c3
            flag_out = 1

         end if

         if ( flag_out .ne. 0 ) then
            count_grid = count_grid + 1
         else if ( bdf_line(1:6) .eq. 'CTRIA3' ) then
            count_elem = count_elem + 1
         end if

         

c        Checking whether or not the grid is on the priodic boundary
c        Storeing grids on the priodic boundary to C*_BM*****  
         if ( flag_out .ne. 0 ) then
            flag_out = 0

            if ( grid_c2 .le. c2_min + c_error )  then
               c2_bmin_cnt = c2_bmin_cnt + 1
               c2_bmin( 1, c2_bmin_cnt ) = grid_c1
               c2_bmin( 2, c2_bmin_cnt ) = c2_min
               c2_bmin_no( c2_bmin_cnt ) = grid_no

               if ( grid_c3 .le. c3_min + c_error ) then
                  c2_bmin( 3, c2_bmin_cnt ) = c3_min
                  cp_cnt1 = cp_cnt1 + 1
                  cp_no( 1, cp_cnt1 ) = c2_bmin_cnt
               else if ( grid_c3 .ge. c3_max - c_error ) then
                  c2_bmin( 3, c2_bmin_cnt ) = c3_max
                  cp_cnt2 = cp_cnt2 + 1
                  cp_no( 2, cp_cnt2 ) = c2_bmin_cnt
               else
                  c2_bmin( 3, c2_bmin_cnt ) = grid_c3
               end if


           else if ( grid_c2 .ge. c2_max - c_error )  then
              c2_bmax_cnt = c2_bmax_cnt + 1
              c2_bmax( 1, c2_bmax_cnt ) = grid_c1
              c2_bmax( 2, c2_bmax_cnt ) = c2_max
              c2_bmax_no( c2_bmax_cnt ) = grid_no

              if ( grid_c3 .le. c3_min + c_error ) then
                  c2_bmax( 3, c2_bmax_cnt ) = c3_min
                  cp_cnt4 = cp_cnt4 + 1
                  cp_no( 4, cp_cnt4 ) = c2_bmax_cnt
              else if ( grid_c3 .ge. c3_max - c_error ) then
                  c2_bmax( 3, c2_bmax_cnt ) = c3_max
                  cp_cnt3 = cp_cnt3 + 1
                  cp_no( 3, cp_cnt3 ) = c2_bmax_cnt
              else
                  c2_bmax( 3, c2_bmax_cnt ) = grid_c3
              end if


            else if ( grid_c3 .le. c3_min + c_error )  then
               c3_bmin_cnt = c3_bmin_cnt + 1
               c3_bmin( 1, c3_bmin_cnt ) = grid_c1
               c3_bmin( 2, c3_bmin_cnt ) = grid_c2
               c3_bmin( 3, c3_bmin_cnt ) = c3_min
               c3_bmin_no( c3_bmin_cnt ) = grid_no
c               write(10,*)c3_bmin_cnt,(c3_bmin(j,c3_bmin_cnt),j=1,3),
c     $              grid_no

            else if ( grid_c3 .ge. c3_max - c_error )  then
               c3_bmax_cnt = c3_bmax_cnt + 1
               c3_bmax( 1, c3_bmax_cnt ) = grid_c1
               c3_bmax( 2, c3_bmax_cnt ) = grid_c2
               c3_bmax( 3, c3_bmax_cnt ) = c3_max
               c3_bmax_no( c3_bmax_cnt ) = grid_no
c               write(11,*)c3_bmax_cnt,(c3_bmax(j,c3_bmax_cnt),j=1,3),
c     $              grid_no

            end if


         end if
         
         if (bdf_line(1:7) == 'ENDDATA') exit
       end do


9101   close(91)

c      Overwriteing cordinate 1,3 of the grids on the right priodic boundary
       if ( c2_bmin_cnt .eq. c2_bmax_cnt ) then
          do i=1 , c2_bmin_cnt
             do j=1 , c2_bmax_cnt
                if (c2_bmax(1,j) .le. c2_bmin(1,i) + c_error) then
                 if (c2_bmax(1,j) .ge. c2_bmin(1,i) - c_error) then         
                  if (c2_bmax(3,j) .le. c2_bmin(3,i) + c_error) then
                   if (c2_bmax(3,j) .ge. c2_bmin(3,i) - c_error) then
                      c2_bmax(1,j) = c2_bmin(1,i)
                      c2_bmax(3,j) = c2_bmin(3,i)
                   end if
                  end if
                 end if
                endif
             end do
          end do
       else
          write(*,*) '2 cordinate boundary error'
          write(*,*)
     &    'number of grids on the left  side:' , c2_bmin_cnt
          write(*,*)
     &    'number of grids on the right side:' , c2_bmax_cnt
          stop
       end if

c      Overwriteing cordinate 1,3 of the grids on the right priodic boundary 
       if ( c3_bmin_cnt .eq. c3_bmax_cnt ) then
          do i=1 , c3_bmin_cnt
             do j=1 , c3_bmax_cnt
                if (c3_bmax(1,j) .le. c3_bmin(1,i) + c_error) then
                 if (c3_bmax(1,j) .ge. c3_bmin(1,i) - c_error) then
                  if (c3_bmax(2,j) .le. c3_bmin(2,i) + c_error) then
                   if (c3_bmax(2,j) .ge. c3_bmin(2,i) - c_error) then
                      c3_bmax(1,j) = c3_bmin(1,i)
                      c3_bmax(2,j) = c3_bmin(2,i)
                   end if
                  end if
                 end if
                endif
             end do
           end do
      else
          write(*,*) '3 cordinate boundary error'
          write(*,*) 
     &    'number of grids on the left  side:' , c3_bmin_cnt
          write(*,*)
     &    'number of grids on the right side:' , c3_bmax_cnt
          stop
       end if


       do i=1 , cp_cnt1
          do j=1 , cp_cnt2
             if    (c2_bmin(1,cp_no(2,j)) .le. 
     &              c2_bmin(1,cp_no(1,i)) + c_error) then
                if (c2_bmin(1,cp_no(2,j)) .ge. 
     &              c2_bmin(1,cp_no(1,i)) - c_error) then
                   c2_bmin(1,cp_no(2,j)) = c2_bmin(1,cp_no(1,i))
                end if
             end if
          end do
       end do

       do i=1 , cp_cnt4
          do j=1 , cp_cnt3
             if    (c2_bmax(1,cp_no(3,j)) .le.
     &              c2_bmax(1,cp_no(4,i)) + c_error) then
                if (c2_bmax(1,cp_no(3,j)) .ge.
     &              c2_bmax(1,cp_no(4,i)) - c_error) then
                   c2_bmax(1,cp_no(3,j)) = c2_bmax(1,cp_no(4,i))
                end if
             end if
          end do
       end do


c      Opening the bdf file
       open(91,FILE=bdf_file)
c      Opening the output file
       open(81,FILE='fmm_b.dat')
c      Opening the changed bdf file
       open(82,FILE='fmm_'//bdf_file)
       i=1
       j=1
       k=1
       l=1
       flag_out=0


       write(81,'(3(i7,2x))') count_elem , count_grid , layer_num

       do
         read(91,'(A72)',end=9102) bdf_line

c        Reading the relation betwenn element and grid.
         if (bdf_line(1:6) .eq. 'CTRIA3') then
c        Transration from the character value to the numelical value
            read( bdf_line(10:17) , * ) elem_no
            read( bdf_line(18:25) , * ) shee_no
            read( bdf_line(26:33) , * ) grid_no1
            read( bdf_line(34:41) , * ) grid_no2
            read( bdf_line(42:49) , * ) grid_no3

c        element data output
            count_elem_chk = count_elem_chk+1

            if ( elem_no .ne. count_elem_chk ) then
               write(*,*) 'elemment reading error'
     &                    ,elem_no,count_elem_chk
               stop
            end if

c            write(81,*) 
            write(81,'(4(i6,1x),2(i3,1x))')
     &      elem_no , grid_no1 , grid_no2 , grid_no3
     &              , elem_nv(2,shee_no)  , elem_nv(1,shee_no)

c        Reading cordinates of the grid
         else if ( bdf_line(1:5) .eq. 'GRID ' ) then
c        Reading cordinates of the grid
c        Transration from the character value to the numerical value
            read( bdf_line(10:24) , * ) grid_no
            read( bdf_line(25:32) , * ) grid_c1
            read( bdf_line(33:40) , * ) grid_c2
            read( bdf_line(41:48) , * ) grid_c3
            flag_out = 1

         else if (bdf_line(1:5) .eq. 'GRID*' ) then
            read( bdf_line(10:24) , * ) grid_no
            read( bdf_line(41:56) , * ) grid_c1
            read( bdf_line(57:72) , * ) grid_c2

            read(91,'(a72)',end=9102) bdf_line

            read( bdf_line(9:24)  , * ) grid_c3
            flag_out = 1

         end if

         if ( flag_out .ne. 0 ) then

            if (grid_no .eq. c2_bmin_no(i)) then
               write(*,'(a5,2x,i6,1x,4F15.9)')
     &              'c2min',grid_no
     &              ,c2_bmin(1,i),c2_bmin(2,i),c2_bmin(3,i)
     &              ,( (grid_c1-c2_bmin(1,i))**2
     &               + (grid_c2-c2_bmin(2,i))**2
     &               + (grid_c3-c2_bmin(3,i))**2 )**0.5
               grid_c1 = c2_bmin(1,i)
               grid_c2 = c2_bmin(2,i)
               grid_c3 = c2_bmin(3,i)
               i = i + 1
c               flag_out=2
            else if (grid_no .eq. c2_bmax_no(j)) then
               write(*,'(a5,2x,i6,1x,4F15.9)')
     &              'c2max',grid_no
     &              ,c2_bmax(1,j),c2_bmax(2,j),c2_bmax(3,j)
     &              ,( (grid_c1-c2_bmax(1,j))**2
     &               + (grid_c2-c2_bmax(2,j))**2
     &               + (grid_c3-c2_bmax(3,j))**2 )**0.5
               grid_c1 = c2_bmax(1,j)
               grid_c2 = c2_bmax(2,j)
               grid_c3 = c2_bmax(3,j)
               j = j + 1
c               flag_out=2
            else if (grid_no .eq. c3_bmin_no(k)) then
               write(*,'(a5,2x,i6,1x,4F15.9)') 
     &              'c3min',grid_no
     &              ,c3_bmin(1,k),c3_bmin(2,k),c3_bmin(3,k)
     &              ,( (grid_c1-c3_bmin(1,k))**2
     &               + (grid_c2-c3_bmin(2,k))**2
     &               + (grid_c3-c3_bmin(3,k))**2 )**0.5
               grid_c1 = c3_bmin(1,k)
               grid_c2 = c3_bmin(2,k)
               grid_c3 = c3_bmin(3,k)
               k = k + 1
c               flag_out=2
            else if (grid_no .eq. c3_bmax_no(l)) then
               write(*,'(a5,2x,i6,1x,4F15.9)') 
     &              'c3max',grid_no
     &              ,c3_bmax(1,l),c3_bmax(2,l),c3_bmax(3,l)
     &              ,( (grid_c1-c3_bmax(1,l))**2
     &               + (grid_c2-c3_bmax(2,l))**2
     &               + (grid_c3-c3_bmax(3,l))**2 )**0.5
               grid_c1 = c3_bmax(1,l)
               grid_c2 = c3_bmax(2,l)
               grid_c3 = c3_bmax(3,l)
               l = l + 1
c               flag_out=2
            end if

c        grid data output

            count_grid_chk=count_grid_chk+1
            if ( grid_no .ne. count_grid_chk ) then
               write(*,*) 'grid reading error'
     &                    ,grid_no,count_grid_chk
               stop
            end if

c            write(81,*)
            write(81,'(i6,1x,3(e20.9,1x))') 
     &            grid_no , grid_c1 , grid_c2 , grid_c3
         end if   


c        bdf data output
         if ( flag_out .ne. 0 ) then
            flag_out = 0
            write(82,'(a,3x,i7,25x,2(e11.4,5x),/,a,7x,e11.4)') 
     &            'GRID*',grid_no,grid_c1,grid_c2,'*',grid_c3
         else
            write(82,'(a72)') bdf_line
         end if



         if (bdf_line(1:7) == 'ENDDATA') exit
       end do  

 9102  close(91)
 8101  close(81)
 8102  close(82)
       write(*,*) 'DONE ...'
       end

 
